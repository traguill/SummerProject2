using UnityEngine;
using System.Collections;

public class DeathTrap : MonoBehaviour {

    bool active = false; //The trap has activate?


    public GameObject idle;
    public GameObject explosion;

    [HideInInspector]public NyxDeathTrapState nyx_death_trap_state; //Reference to the nyx state

    public float kill_radius = 3; //Radius of the zone that will kill every enemy inside
    public LayerMask enemy_layer; //Layer mask of enemies

    EnemyManager enemy_manager;

    GameObject idle_anim;
    GameObject explosion_anim;

    public float explosion_duration = 1.5f;
    float timer_explosion = 0.0f;

    void Awake()
    {
        idle_anim = Instantiate(idle);
        idle_anim.transform.SetParent(transform);
        idle_anim.transform.localPosition = new Vector3(0, 0, 0);
        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }


	
	// Update is called once per frame
	void Update () 
    {
        //The trap is activated, wait for finish explosion animation and destroy it.
	    if(active)
        {
            timer_explosion += Time.deltaTime;
            if(timer_explosion >= explosion_duration)
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

            Destroy(idle_anim);

            explosion_anim = Instantiate(explosion);
            explosion_anim.transform.SetParent(transform);
            explosion_anim.transform.localPosition = new Vector3(0, 0, 0);

            //Kill the enemy
            Collider[] enemies = Physics.OverlapSphere(transform.position, kill_radius, enemy_layer);

            foreach(Collider enemy in enemies)
            {
                enemy_manager.DestroyEnemy(enemy.gameObject);
            }
        }
    }
}
