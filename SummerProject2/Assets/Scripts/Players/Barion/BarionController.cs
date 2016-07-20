using UnityEngine;
using System.Collections;

public class BarionController : MonoBehaviour {

    enum State
    {
        IDLE,
        WALKING,
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

    void Start()
    {
        state = State.IDLE;

        selection_circle.SetActive(false);
        is_selected = false;
    }


    void Update ()
    {
        UpdateSelection();

        switch(state)
        {
            case State.IDLE:
                break;
            case State.WALKING:
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

    //MOVE BOX ------------------------------------------------------------
    public void BoxUp()
    {
        print("box up");
    }

    public void BoxRight()
    {
        print("box right");
    }

    public void BoxDown()
    {
        print("box down");
    }

    public void BoxLeft()
    {
        print("box left");
    }
}
