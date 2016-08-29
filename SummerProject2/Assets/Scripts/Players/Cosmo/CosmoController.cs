using UnityEngine;
using System.Collections;

public class CosmoController : MonoBehaviour {

    //Players manager
    [HideInInspector] public PlayersManager players_manager;

    //Selection
    public GameObject selection_circle;
    [HideInInspector]public UnitSelection selection_system;
    [HideInInspector] public bool is_selected = false;

    //Navigation
    [HideInInspector] public NavMeshAgent agent;
    public LayerMask raycast_layers; //Available layers for raycasting (ingore fog of war).
    public LayerMask movement_layers; //Layers that the raycast must hit to start movement.

    //State machine
    [HideInInspector] ICosmoState current_state;
    [HideInInspector] public CosmoIdleState idle_state;
    [HideInInspector] public CosmoWalkingState walking_state;
    [HideInInspector] public CosmoSensorialState sensorial_state;
    [HideInInspector] public CosmoHidingState hiding_state;
    [HideInInspector] public CosmoPortalState portal_state;   
    [HideInInspector] public CosmoChainedState chained_state;

    [HideInInspector] public bool is_hide; 

     //Box interaction
   [HideInInspector] public GameObject target_box = null; //Box to interact with

     //Abilities cooldown
    [HideInInspector] public Cooldown cooldown_inst; //Instance to cooldown script

    //Abilities utils
    //***************************************************************************************
    //Portal
    public PortalController portal_controller_prefab; //Controller of both portals
    public LayerMask portal_build_layers; //Layers to collide when raycasting for building portals. (Everithing except fog of war)
    public LayerMask floor_layer; //Floor layer
    public GameObject build_portal_prefab; //Portal image to show when building

    //Sensorial
    public float sensorial_cast_time = 1.0f;
    public float max_detection_radius = 20.0f;
    public GameObject sensorial_anim_prefab;

    void Awake()
    {
        players_manager = GetComponentInParent<PlayersManager>();
        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
        cooldown_inst = GetComponent<Cooldown>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        //State machine
        idle_state = new CosmoIdleState(this);
        walking_state = new CosmoWalkingState(this);
        sensorial_state = new CosmoSensorialState(this);
        hiding_state = new CosmoHidingState(this);
        portal_state = new CosmoPortalState(this);
        Transform barion = GameObject.Find(Objects.barion).transform;
        chained_state = new CosmoChainedState(this, barion);
    }

	// Use this for initialization
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
    public void ChangeStateTo(ICosmoState new_state)
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

    //Box interaction -----------------------------------------------------------------------

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
    /// This method is called when Cosmo is selected and hided in a box.
    /// </summary>
    public void HideSelected()
    {
        selection_system.AutoSelectPlayer(gameObject);
    }

    /// <summary>
    /// Returns the current state.
    /// </summary>
    public ICosmoState GetState()
    {
        return current_state;
    }
   
}
