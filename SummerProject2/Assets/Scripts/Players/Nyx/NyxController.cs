using UnityEngine;
using System.Collections;

public class NyxController : MonoBehaviour {

    //Players manager
    [HideInInspector] public PlayersManager players_manager;

    //Selection
    public GameObject selection_circle;
    [HideInInspector] public UnitSelection selection_system;
    [HideInInspector] public bool is_selected = false;

    //Entity Manager
    [HideInInspector] public EnemyManager enemy_manager;

    //Navigation
    [HideInInspector]
    public NavMeshAgent agent;
    public LayerMask raycast_layers; //Available layers for raycasting (ingore fog of war).
    public LayerMask movement_layers; //Layers that the raycast must hit to start movement.
   

    //State machine
    [HideInInspector] INyxState current_state;
    [HideInInspector] public NyxIdleState idle_state;
    [HideInInspector] public NyxWalkingState walking_state;
    [HideInInspector] public NyxKillingState killing_state;
    [HideInInspector] public NyxHidingState hiding_state;
    [HideInInspector] public NyxDashState dash_state;
    [HideInInspector] public NyxDeathTrapState death_trap_state;
    [HideInInspector] public NyxChainedState chained_state;

    //Killing
    [HideInInspector] public GameObject target_to_kill; //Enemy marked to kill with passive.
    public float death_trap_range = 5;

    //Dash
    public LayerMask floor_layer; //Floor layer (for dash position for now)
    public float dash_speed = 100; //Speed of the dash
    public LayerMask dash_collision_layer; //Layers that will stop the dash on collision
    public float dash_range = 20; //Maximum range of the dash ability

    //DeathTrap
    public GameObject death_trap_prefab;

    [HideInInspector]
    public bool is_hide;

    //Box interaction
    [HideInInspector]
    public GameObject target_box = null; //Box to interact with

    //Abilities cooldown
    [HideInInspector] public Cooldown cooldown_inst; //Instance to cooldown script

    void Awake()
    {
        players_manager = GetComponentInParent<PlayersManager>();

        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        cooldown_inst = GetComponent<Cooldown>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        idle_state = new NyxIdleState(this);
        walking_state = new NyxWalkingState(this);
        killing_state = new NyxKillingState(this);
        hiding_state = new NyxHidingState(this);
        dash_state = new NyxDashState(this);
        death_trap_state = new NyxDeathTrapState(this);
        Transform barion_transform = GameObject.Find("Barion").transform;
        chained_state = new NyxChainedState(this, barion_transform);
    }

    void Start ()
    {
        selection_circle.SetActive(false);
        is_selected = false;

        current_state = idle_state;
        is_hide = false;
    }
	
	void Update ()
    {
        UpdateSelection();

        current_state.UpdateState();
    }

    /// <summary>
    /// Checks if the Player is selected and updates the selection circle.
    /// </summary>
    private void UpdateSelection()
    {
        is_selected = selection_system.IsPlayerSelected(gameObject);

        selection_circle.SetActive(is_selected);
    }

    /// <summary>
    /// stopMovement finishes all pathFinding activity
    /// </summary>
    public void StopMovement()
    {
        agent.Stop();
        agent.ResetPath();
    }

    //STATE MACHINE UTILS ------------------------------------------------------------------------

    /// <summary>
    /// Transition to the passed state and resets all the new state variables.
    /// </summary>
    public void ChangeStateTo(INyxState new_state)
    {
        current_state = new_state;
        current_state.StartState();
    }

    /// <summary>
    /// Checks if walking action is performed. If so, destination parameter is filed.
    /// </summary>
    public bool GetMovement(ref Vector3 destination)
    {
        if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, raycast_layers))
            {
                //Hit is in a movment layer(floor, environment,etc)?
                if (movement_layers == (movement_layers | (1 << hit.transform.gameObject.layer)))
                {
                    destination = hit.point;
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Has arrived at the destination point?
    /// </summary>
    public bool DstArrived()
    {
        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Returns if an enemy is selected to kill (as passive)
    /// </summary>
    public bool KillEnemy()
    {
        if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, raycast_layers))
            {
                if (movement_layers == (movement_layers | (1 << hit.transform.gameObject.layer)))
                {
                    if(hit.transform.tag == Tags.enemy) //Check if the hitted point is an enemy (alive)
                    {

                        target_to_kill = hit.transform.gameObject;
                        return true;
                    }
                  
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the current state.
    /// </summary>
    public INyxState GetState()
    {
        return current_state;
    }

    //Box interaction -------------------------------------------------------------------------------------------------------


    /// <summary>
    /// This method is called when the player wants to hide inside the box. Note: when one player wants to hide the 3 players recieve the call to this method (we discard the action if the player is not selected)
    /// </summary>
    public void HideInBox(GameObject box)
    {
        if (is_selected)
        {
            target_box = box;
            ChangeStateTo(hiding_state);
        }
    }

    /// <summary>
    /// This method is called when Barion is selected and hided in a box.
    /// </summary>
    public void HideSelected()
    {
        selection_system.AutoSelectPlayer(gameObject);
    }

}
