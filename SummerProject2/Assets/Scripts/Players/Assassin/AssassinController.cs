using UnityEngine;
using System.Collections;

public class AssassinController : MonoBehaviour {

    enum State
    {
        IDLE,
        WALKING,
        WALKING_TO_KILL, //Approaching to selected enemy to kill it
        KILLING //Action to kill
    };

    State state;

    //Selection
    public GameObject selection_circle;
    UnitSelection selection_system;
    bool is_selected = false;

    //Navigation
    NavMeshAgent agent;

    //Killing
    private string kill_point_name = "KillPoint"; //Name of gameobject child from enemy (destination point to kill the enemy)
    GameObject target_to_kill;
    EnemyManager enemy_manager;

    public LayerMask raycast_layer;

    void Awake()
    {
        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    void Start ()
    {
        state = State.IDLE;

        selection_circle.SetActive(false);
        is_selected = false;
    }
	
	void Update ()
    {
        UpdateSelection();

        InputMouse();

        switch(state)
        {
            case State.IDLE:
                break;
            case State.WALKING:
                WalkingState();
                break;
            case State.WALKING_TO_KILL:
                WalkingToKillState();
                break;
            case State.KILLING:
                KillingState();
                break;
        }
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
    public void stopMovement()
    {
        agent.Stop();
        agent.ResetPath();
    }

    /// <summary>
    /// Handles all the logic of a mouse input (movement, objectives, objects, etc).
    /// </summary>
    private void InputMouse()
    {
        if (is_selected == false)
            return;

        //Right click UP
        if (Input.GetMouseButtonUp(1))
        {
            //Check what is under the mouse (enemy, object, nothing...)

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, raycast_layer))
            {
                if(hit.transform.tag == Tags.enemy)
                {
                    //Search kill-point
                    Transform kill_point = hit.transform.FindChild(kill_point_name);
                    if(kill_point != null)
                    {
                        agent.SetDestination(kill_point.transform.position);
                        target_to_kill = hit.transform.gameObject;
                        state = State.WALKING_TO_KILL;               
                    }
                    else
                    {
                        print("Enemy: " + hit.transform.name + " doesn't have a Kill Point");
                    }
                }
                else
                {       //Movement
                    agent.SetDestination(hit.point);
                    state = State.WALKING;
                }
               
            }
        }
    }

    private bool ReachedDestination()
    {
        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance)
            return true;
        else
            return false;
    }

    //STATE MACHINE LOGIC ----------------------------------------------------------------------------------------
    private void WalkingState()
    {
        if (ReachedDestination())
            state = State.IDLE;
    }

    private void WalkingToKillState()
    {
        if (ReachedDestination())
            state = State.KILLING;
    }

    private void KillingState()
    {
        enemy_manager.KillEnemy(target_to_kill, true);
        state = State.IDLE;
    }
}
