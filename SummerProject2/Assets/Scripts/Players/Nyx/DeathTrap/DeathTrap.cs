using UnityEngine;
using System.Collections;

public class DeathTrap : MonoBehaviour {

    bool active = false; //The trap has activate?


    ParticleSystem mark; //Mark on the floor
    ParticleSystem explosion; //Explosion of the trap

    [HideInInspector]public NyxDeathTrapState nyx_death_trap_state; //Reference to the nyx state

    public float kill_radius = 3; //Radius of the zone that will kill every enemy inside
    public LayerMask enemy_layer; //Layer mask of enemies

    EnemyManager enemy_manager;

    void Awake()
    {
        mark = transform.FindChild("Mark").GetComponent<ParticleSystem>();
        explosion = transform.FindChild("Explosion").GetComponent<ParticleSystem>();
        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }


	
	// Update is called once per frame
	void Update () 
    {
        //The trap is activated, wait for finish explosion animation and destroy it.
	    if(active)
        {
            if (explosion.IsAlive() == false)
            {
                DestroyDeathTrap(null);
            }
               
        }
	}

    /// <summary>
    /// Destroys the death trap. The parameter is necesary for the HUD, if it's not called from there pass null.
    /// </summary>
    public void DestroyDeathTrap(GameObject obj)
    {
        nyx_death_trap_state.trap = null;
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == Tags.enemy && active == false) //Change for enemy
        {
            nyx_death_trap_state.trap = null; //Nyx has no control over the trap when it has been activated.
            active = true;

            mark.Stop();
            explosion.Play();

            //Kill the enemy
            Collider[] enemies = Physics.OverlapSphere(transform.position, kill_radius, enemy_layer);

            foreach(Collider enemy in enemies)
            {
                enemy_manager.DestroyEnemy(enemy.gameObject);
            }
        }
    }
}
