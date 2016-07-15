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

	void Start ()
    {
        state = State.IDLE;
	}
	
	void Update ()
    {
	
	}
}
