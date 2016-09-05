using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PATROL_TYPE
{
    NEUTRAL,
    ALERT
};

/* Patrol class contains all the related information to proper execute the different patrols. Rhadnor controller
 * will deal with two: neutral (without alarm) and alert (with alarm). Both patrols can use the same path but different
 * configuration between them can be applied;
 */

[System.Serializable]
public class Patrol
{
    public PATROL_TYPE type;                    // Type of the patrol: NEUTRAL or ALERT
    public bool expanded;                       // For Toggle Editor use, useful to expand the different patrol info

    public bool static_patrol;                  // Rhandor will remain on its initial position without doing patrols
    public bool loop;                           // Rhandor will change its direction upon reaching last position and the same 
                                                // to the first position. If false, Rhandor will close its patrol path.

    public bool is_synchronized;                // Declares a synchronized patrol with ONLY other Rhandor (so far)
    public GameObject synchronized_Rhandor;     // The Rhandor synchronized with
    public int give_permission_pos;             // Position where Rhandor will give permission to move to the synchronized Rhandor.
    public int ask_for_permission_pos;          // Position where Rhandor will ask for permission to its synchronized Rhandor.
    public bool can_give_permission = false;    // Rhandor will give permissions of movement.
    public bool can_get_permission = false;     // Rhandor will recieve permissions to resume its movement.

    public GameObject path_attached;            // The GameObject that contains waypoints as childs and represents the patrol path.

    public int size;                            // Number of waypoints
    public Vector3[] path;                      // The colection of positions that conforms the patrol
    public float[] stop_times;                  // Number of seconds that the enemy will stop at the selected position
    public bool[] trigger_movement;             // This position will trigger the movement of the synchronized Rhandor
    public bool[] recieve_trigger;              // Response to a trigger movement from the synchronzied Rhandor

    public Patrol(int _size, PATROL_TYPE _type)
    {
        size = _size;
        type = _type;

        path = new Vector3[_size];
        stop_times = new float[_size];
        trigger_movement = new bool[_size];
        recieve_trigger = new bool[_size];
    }

    public Patrol(Patrol patrol)
    {
        size = patrol.size;
        type = patrol.type;

        path = patrol.path;
        stop_times = patrol.stop_times;
        trigger_movement = patrol.trigger_movement;
        recieve_trigger = patrol.recieve_trigger;
    }

    public void Set(Patrol patrol)
    {
        size = patrol.size;
        type = patrol.type;

        path = patrol.path;
        stop_times = patrol.stop_times;
        trigger_movement = patrol.trigger_movement;
        recieve_trigger = patrol.recieve_trigger;
    }

    // Property
    public int Length
    {
        get
        {
            return size;
        }
    }
}

public class RhandorController : Enemies {

    
    public float patrol_speed, alert_speed, spotted_speed;   // Speed for the different patrols
    private bool inverse_patrol;                             // Determines patrol direction when loop boolean is active
    public bool same_neutral_alert_path;                     // Neutral and alert path is exactly the same.

    public Patrol neutral_patrol, alert_patrol;   

    public float time_waiting_on_position;      
    public int current_position;
    public Vector3 initial_position, initial_forward_direction;
    public Quaternion initial_orientation;
    private float ground_level;                              // ground correction for proper visualization of patrols

    private float time_recovering_timer, max_time_recovering;

    // When something is identified, the enemy will ask for help...
    public float ask_for_help_radius;
    public int max_num_of_helpers;

    // Synchronicity
    public bool movement_allowed = false;
    public bool permission_given = false;
    public bool waiting_permission = false;

    // For corpses representation
    [HideInInspector] public SpriteRenderer render;

    //State machine
    [HideInInspector] IRhandorStates current_state;
    [HideInInspector] public RhandorIdleState idle_state;
    [HideInInspector] public RhandorPatrolState patrol_state;
    [HideInInspector] public RhandorAlertState alert_state;
    [HideInInspector] public RhandorSpottedState spotted_state;
    [HideInInspector] public RhandorSupportState support_state;
    [HideInInspector] public RhandorCorpseState corpse_state;

    // Scripts references
    [HideInInspector] public AlarmSystem alarm_system;
    [HideInInspector] public LastSpottedPosition last_spotted_position;
    [HideInInspector] public EnemyFieldView enemy_field_view;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public EnemyManager enemy_manager;

    // --- Menus ---
    //List of menus that enemy will display with different interactions.
    Dictionary<string, RadialMenu_ObjectInteractable> menus = new Dictionary<string, RadialMenu_ObjectInteractable>(); 
    private string carry_id = "Carry";
    private string drop_id = "Drop";

    void Awake()
    {
        // State machine
        // -- IDLE --
        idle_state = new RhandorIdleState(this);
        // -- PATROL --
        patrol_state = new RhandorPatrolState(this);
        LoadNeutralPatrol();
        // -- ALERT --
        alert_state = new RhandorAlertState(this);
        LoadAlertPatrol();
        // -- SPOTTED --
        spotted_state = new RhandorSpottedState(this);
        // -- SPOTTED --
        support_state = new RhandorSupportState(this);
        // -- CORPSE --
        corpse_state = new RhandorCorpseState(this);

        if(neutral_patrol.is_synchronized)
            LoadSynchronousConfiguration(neutral_patrol);

        if (alert_patrol.is_synchronized)
            LoadSynchronousConfiguration(alert_patrol);

        if (neutral_patrol.path_attached == alert_patrol.path_attached)
            same_neutral_alert_path = true;

        render = GetComponent<SpriteRenderer>();
        type = ENEMY_TYPES.RHANDOR;

        initial_forward_direction = transform.forward;
        time_recovering_timer = max_time_recovering = 2.0f;

        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        last_spotted_position = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<LastSpottedPosition>();
        enemy_field_view = GetComponent<EnemyFieldView>();
        enemy_manager = GetComponentInParent<EnemyManager>();   //Every enemy should be child of the enemy manager
        agent = GetComponent<NavMeshAgent>();                   // Agent for NavMesh

        //Insert all menus in the dictionary
        RadialMenu_ObjectInteractable[] menus_scripts = GetComponents<RadialMenu_ObjectInteractable>();
        foreach (RadialMenu_ObjectInteractable menu_script in menus_scripts)
        {
            menus.Add(menu_script.id, menu_script);
        }
    }

    void Start()
    {
        if (neutral_patrol.static_patrol)
            ChangeStateTo(idle_state);
        else
            ChangeStateTo(patrol_state);         

        // DEBUG
        if (tag.Equals(Tags.corpse))
            ChangeStateTo(corpse_state);
    }

    void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.X))
            KillRhandor();

        current_state.UpdateState();
    }

    public void ChangeStateTo(IRhandorStates new_state)
    {
        current_state = new_state;
        current_state.StartState();
    }

    public void Dead()
    {
        if (current_state != corpse_state)
            ChangeStateTo(corpse_state);
        else
            Debug.Log("The enemy is already dead!");
    }

    /// <summary>
    /// Returns the current state.
    /// </summary>
    public IRhandorStates GetState()
    {
        return current_state;
    }

    /// <summary>
    /// goToNextPoint gives the index for the next position of the argument current path
    /// </summary>
    /// <returns> The index of the next position </returns>
    public void goToNextPoint(Vector3[] current_path, bool is_path_looped)
    {
        // Returns if no points have been set up
        if (current_path.Length == 0)
            return;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary or returning if a loop is considered.
        if(is_path_looped)
        {
            if(inverse_patrol)
            {
                if (current_position == 0)
                {
                    current_position += 1;
                    inverse_patrol = false;
                }
                else
                    current_position -= 1;                
            }
            else
            {
                if (current_position == current_path.Length - 1)
                {
                    current_position -= 1;
                    inverse_patrol = true;
                }
                else
                    current_position += 1;                
            }
        }            
        else        
            current_position = (current_position + 1) % current_path.Length;

        // Set the agent to go to the currently selected destination.
        agent.SetDestination(current_path[current_position]);        
    }

    private void KillRhandor()
    {
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Enemy", "Corpse")))
            {
                if (hit.collider == GetComponent<CapsuleCollider>())
                {
                    if(current_state != corpse_state)                    
                        ChangeStateTo(corpse_state);
                    else
                    {
                        tag = Tags.enemy;
                        gameObject.layer = LayerMask.NameToLayer("Enemy");
                        agent.enabled = true;

                        if (neutral_patrol.static_patrol)
                            ChangeStateTo(idle_state);
                        else
                            ChangeStateTo(patrol_state);
                    }
                }
            }
        }
    }

    /// <summary>
    /// findClosestPoint finds the nearest position from the current patrol path
    /// assigned to this enemy. It is used when the game switches between patroling states.
    /// </summary>
    /// <returns> The index of the closest position </returns>
    public int findClosestPoint(Vector3[] path_to_search)
    {
        int index = -1;
        NavMeshPath path = new NavMeshPath();
        float minimum_distance = 1000000.0f;

        for (int i = 0; i < path_to_search.Length; ++i)
        {
            agent.CalculatePath(path_to_search[i], path);
            float distance = 0;
            for (int j = 0; j < path.corners.Length - 1; ++j)
            {
                distance += Vector3.Distance(path.corners[j], path.corners[j + 1]);
            }

            if (distance < minimum_distance)
            {
                index = i;
                minimum_distance = distance;
            }
        }

        return index;
    }

    private void GivePermission(Patrol patrol)
    {
        if (patrol.synchronized_Rhandor.GetComponent<RhandorController>().waiting_permission)
        {
            if (current_position == patrol.give_permission_pos)
            {
                permission_given = true;
                patrol.synchronized_Rhandor.GetComponent<RhandorController>().movement_allowed = true;
                Debug.Log(name + " gives permission to " + patrol.synchronized_Rhandor.GetComponent<RhandorController>().name);
            }
        }                     
    }

    private void RecievePermission(Patrol patrol)
    {
        if (current_position == patrol.ask_for_permission_pos && !patrol.synchronized_Rhandor.GetComponent<RhandorController>().waiting_permission)
        {
            waiting_permission = true;
            Debug.Log(name + " wants to move");
        }
            
    }

    /// <summary>
    ///  Check if the enemy has to change its destination or has to wait the number of seconds the user
    ///  has introduced on the current position.
    /// </summary>
    public void CheckNextMovement(Patrol current_patrol)
    {
        //Choose the next destination point when the agent gets close to the current one.
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            if (current_patrol.is_synchronized)
            {
                if (current_patrol.can_give_permission && !permission_given) GivePermission(current_patrol);
                if (current_patrol.can_get_permission && !waiting_permission) RecievePermission(current_patrol);               
            }            

            if(waiting_permission)
            {
                if(movement_allowed)
                {
                    goToNextPoint(current_patrol.path, current_patrol.loop);
                    current_patrol.synchronized_Rhandor.GetComponent<RhandorController>().permission_given = false;
                    movement_allowed = false;
                    waiting_permission = false;
                }                           
            }
            else
            {
                if (time_waiting_on_position > current_patrol.stop_times[current_position])
                {
                    goToNextPoint(current_patrol.path, current_patrol.loop);
                    time_waiting_on_position = 0.1f;
                    agent.Resume();
                }
                else
                {
                    time_waiting_on_position += Time.deltaTime;
                    agent.Stop();
                }
            }                           
        }
    }

    public bool RecoveringInitialPosition()
    {
        float initial_dir = initial_orientation.eulerAngles.y;
        float current_dir = transform.rotation.eulerAngles.y;

        float speed_y = (initial_dir - current_dir) / time_recovering_timer;

        transform.eulerAngles += new Vector3(0.0f, speed_y * Time.deltaTime, 0.0f);
        time_recovering_timer -= Time.deltaTime;

        current_dir = transform.rotation.eulerAngles.y; // Updating values

        if (Mathf.Abs(initial_dir - current_dir) < 0.1f)
        {
            time_recovering_timer = max_time_recovering;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// ReturnDefaultBehaviour changes the state of the Rhandor in order to return to one of
    /// the three basic states: IDLE, PATROL or ALERT
    /// </summary>
    public void ReturnDefaultBehaviour()
    {
        if (alarm_system.isAlarmActive())
            ChangeStateTo(alert_state);
        else
        {
            if (neutral_patrol.static_patrol)
                ChangeStateTo(idle_state);
            else
                ChangeStateTo(patrol_state);
        }
    }

    /// <summary>
    /// This method is called when the carry corpses option has been selected.
    /// </summary>
    public void CarryCorpse(GameObject obj)
    {
        if(tag == Tags.corpse)
        {
            enemy_manager.barion.CarryCorpse(gameObject);
        }
        else
        {
            Debug.Log(name + " : Can't carry this corpse because the enemy is not dead");
        }
    }

    public void DropCorpse(GameObject obj)
    {
        if (tag == Tags.corpse)
        {
            enemy_manager.barion.DropCorpse();
        }
        else
        {
            Debug.Log(name + " : Can't drop this corpse because the enemy is not dead");
        }
    }

    /// <summary>
    /// This method is called when this enemy is a corpse and has been selected to carry.
    /// </summary>
    public void CorpseSelected()
    {
        if(enemy_manager.barion.is_selected)
        {
            if(enemy_manager.barion.GetState() != enemy_manager.barion.carry_corpse_state)
            {
                menus[carry_id].OnInteractableClicked(); //Carry
            }
            else
            {
                menus[drop_id].OnInteractableClicked(); //Drop
            }
        }
    }

    public Patrol GetPatrolByType(PATROL_TYPE type)
    {
        Patrol sync_patrol = null;
        switch (type)
        {
            case (PATROL_TYPE.NEUTRAL):
                {
                    sync_patrol = neutral_patrol;
                    break;
                }
            case (PATROL_TYPE.ALERT):
                {
                    sync_patrol = alert_patrol;
                    break;
                }
        }
        return sync_patrol;
    }

    // ---- LOADING METHODS ----
    private void LoadSynchronousConfiguration(Patrol patrol)
    {
        patrol.give_permission_pos = -1;
        for (int i = 0; i < patrol.trigger_movement.Length; ++i)
        {
            if (patrol.trigger_movement[i])
            {
                patrol.give_permission_pos = i;
                break;
            }                
        }

        patrol.can_give_permission = (patrol.give_permission_pos != -1);

        patrol.ask_for_permission_pos = -1;
        for (int i = 0; i < patrol.recieve_trigger.Length; ++i)
        {
            if (patrol.recieve_trigger[i])
            {
                patrol.ask_for_permission_pos = i;
                break;
            }               
        }

        patrol.can_get_permission = (patrol.ask_for_permission_pos != -1);
}

    public void LoadNeutralPatrol()
    {
        ground_level = transform.position.y - (transform.localScale.y / 2);
        initial_position = new Vector3(transform.position.x, ground_level, transform.position.z);
        initial_orientation = transform.rotation;

        // Patrols initialization
        if (!neutral_patrol.static_patrol)
        {
            if (neutral_patrol.path_attached != null)
            {
                // ---- Neutral patrol initialization for editor ----
                Transform[] path = neutral_patrol.path_attached.transform.getChilds();
                if (path.Length > 1)
                {
                    Patrol tmp_patrol = new Patrol(path.Length, PATROL_TYPE.NEUTRAL);

                    for (int i = 0; i < path.Length; ++i)
                    {
                        tmp_patrol.path[i] = path[i].transform.position;
                        tmp_patrol.path[i].y = ground_level;

                        if (neutral_patrol.Length > i)
                        {
                            tmp_patrol.stop_times[i] = neutral_patrol.stop_times[i];
                            tmp_patrol.trigger_movement[i] = neutral_patrol.trigger_movement[i];
                            tmp_patrol.recieve_trigger[i] = neutral_patrol.recieve_trigger[i];
                        }
                    }

                    neutral_patrol.Set(tmp_patrol);
                }
                else              
                    Debug.Log("Error loading NEUTRAL PATROL: The patrol must contain more than one waypoint (use Static toggle instead).");
            }
            else
                Debug.Log("Error loading NEUTRAL PATROL: There is no GameObject attached!");
        }
    }

    public void LoadAlertPatrol()
    {
        // Patrols initialization
        if (!alert_patrol.static_patrol)
        {
            if (alert_patrol.path_attached != null)
            {
                // ---- Alert patrol initialization for editor ----
                Transform[] path = alert_patrol.path_attached.transform.getChilds();
                if (path.Length > 1)
                {
                    Patrol tmp_patrol = new Patrol(path.Length, PATROL_TYPE.ALERT);

                    for (int i = 0; i < path.Length; ++i)
                    {
                        tmp_patrol.path[i] = path[i].transform.position;
                        tmp_patrol.path[i].y = ground_level;

                        if (alert_patrol.Length > i)
                        {
                            tmp_patrol.stop_times[i] = alert_patrol.stop_times[i];
                            tmp_patrol.trigger_movement[i] = alert_patrol.trigger_movement[i];
                            tmp_patrol.recieve_trigger[i] = alert_patrol.recieve_trigger[i];
                        }
                    }

                    alert_patrol.Set(tmp_patrol);
                }
                else
                    Debug.Log("Error loading ALERT PATROL: The patrol must contain more than one waypoint (use Static toggle instead).");
            }
            else
                Debug.Log("Error loading ALERT PATROL: There is no GameObject attached!");
        }
    }
}

