using UnityEngine;
using System.Collections;

public class MentalistController : MonoBehaviour {

    enum State
    {
        IDLE,
        WALKING,
        SENSORIAL //Casting extrasensorial ability
    };

    State state;

    //Selection
    public GameObject selection_circle;
    UnitSelection selection_system;
    bool is_selected = false;

    //Navigation
    NavMeshAgent agent;

    void Awake()
    {
        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

	// Use this for initialization
	void Start ()
    {
        state = State.IDLE;
        
        selection_circle.SetActive(false);
        is_selected = false;
	}
	
	void Update ()
    {
        UpdateSelection();

        AbilitiesInput();

        switch(state)
        {
            case State.IDLE:
                InputMouse();
                break;
            case State.WALKING:
                InputMouse();
                break;
            case State.SENSORIAL:
                 if(GetComponent<SensorialAbility>().is_playing == false)
                {
                    state = State.IDLE;
                }
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

    private void AbilitiesInput()
    {
        if(Input.GetAxis("Ability1") != 0)
        {
            if(GetComponent<SensorialAbility>().is_playing == false)
            {
                GetComponent<SensorialAbility>().UseAbility();

                ChangeState(State.SENSORIAL);
            }           
        }
    }

    /// <summary>
    /// Handles all the logic of a mouse input (movement, objectives, objects, etc).
    /// </summary>
    private void InputMouse()
    {
        //Right click UP
        if(Input.GetMouseButtonUp(1))
        {
            //Check what is under the mouse (enemy, object, nothing...)

            //For now only movment
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.SetDestination(hit.point);
                ChangeState(State.WALKING);
            }
        }
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
    /// Change state (if possible) and resolves all possible conflicts when chaning.
    /// </summary>
    private void ChangeState(State new_state)
    {
        switch(new_state)
        {
            case State.IDLE:
                stopMovement();
                state = new_state;
                break;
            case State.WALKING:
                if (state != State.SENSORIAL)
                    state = new_state;
                break;
            case State.SENSORIAL:
                stopMovement();
                state = new_state;
                break;
        }

        
    }
}
