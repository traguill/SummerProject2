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

	// Use this for initialization
	void Start ()
    {
        state = State.IDLE;
	}
	
	void Update ()
    {
        AbilitiesInput();
	}

    private void AbilitiesInput()
    {
        if(Input.GetAxis("Ability1") != 0)
        {
            if(GetComponent<SensorialAbility>().is_playing == false)
            {
                GetComponent<SensorialAbility>().UseAbility();

            }
           
        }
    }
}
