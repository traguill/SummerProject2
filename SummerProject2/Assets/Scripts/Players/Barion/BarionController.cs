using UnityEngine;
using System.Collections;

public class BarionController : MonoBehaviour {



    //Selection
    public GameObject selection_circle;
    UnitSelection selection_system;
    bool is_selected = false;

    //Navigation
    [HideInInspector] public NavMeshAgent agent;
    public LayerMask raycast_layers; //Available layers for raycasting (ingore fog of war).
    public LayerMask movement_layers; //Layers that the raycast must hit to start movement.

    //State machine
    [HideInInspector] public IBarionState current_state;
    [HideInInspector] public BarionWalkingState walking_state;
    [HideInInspector] public BarionIdleState idle_state;
    [HideInInspector] public BarionMovingBoxState moving_box_state;


    //Move box
    GameObject target_box = null; //Selected box to move.
    Vector3 move_box_position; //Selected location to move the box from.

    void Awake()
    {
        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        //State machine
        idle_state = new BarionIdleState(this);
        walking_state = new BarionWalkingState(this);
        moving_box_state = new BarionMovingBoxState(this);
    }

    void Start()
    {

        selection_circle.SetActive(false);
        is_selected = false;

        current_state = idle_state;
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
    public void stopMovement()
    {
        agent.Stop();
        agent.ResetPath();
    }

    //STATE MACHINE UTILS ------------------------------------------------------------------------

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


    //MOVE BOX ------------------------------------------------------------
    public void BoxUp(GameObject box)
    {
        target_box = box;
        move_box_position = new Vector3(0, 0, 1);
    }

    public void BoxRight(GameObject box)
    {
        target_box = box;
        move_box_position = new Vector3(1, 0, 0);
    }

    public void BoxDown(GameObject box)
    {
        target_box = box;
        move_box_position = new Vector3(0, 0, -1);
    }

    public void BoxLeft(GameObject box)
    {
        target_box = box;
        move_box_position = new Vector3(-1, 0, 0);
    }
}
