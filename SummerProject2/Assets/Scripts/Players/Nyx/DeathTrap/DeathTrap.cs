using UnityEngine;
using System.Collections;

public class DeathTrap : MonoBehaviour {

    bool active = false; //The trap has activate?


    ParticleSystem mark; //Mark on the floor
    ParticleSystem explosion; //Explosion of the trap

    [HideInInspector]public NyxDeathTrapState nyx_death_trap_state; //Reference to the nyx state

    void Awake()
    {
        mark = transform.FindChild("Mark").GetComponent<ParticleSystem>();
        explosion = transform.FindChild("Explosion").GetComponent<ParticleSystem>();
    }


	
	// Update is called once per frame
	void Update () 
    {
        //The trap is activated, wait for finish explosion animation and destroy it.
	    if(active)
        {
            if (explosion.IsAlive() == false)
            {
                nyx_death_trap_state.trap = null;
                Destroy(gameObject);
            }
               
        }
	}

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == Tags.player && active == false) //Change for enemy
        {
            nyx_death_trap_state.trap = null; //Nyx has no control over the trap when it has been activated.
            active = true;

            mark.Stop();
            explosion.Play();

            //Kill the enemy and stop his movement
        }
    }
}
