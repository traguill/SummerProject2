using UnityEngine;
using System.Collections;

public class EnemyFootsteps : MonoBehaviour
{
    GameObject enemy;
    ParticleSystem footsteps;

    LayerMask player_mask;

    public float sound_radius;

    MeshRenderer render; //If the render is enabled the enemy is visible.

    float time_detection = 0.5f; //Check players every 0.5s to optimize the physics
    float timer = 0.0f;

	void Awake ()
    {
        player_mask = LayerMask.GetMask("Player");
        enemy = transform.parent.gameObject;
        footsteps = GetComponent<ParticleSystem>();

        render = enemy.GetComponent<MeshRenderer>();

        footsteps.Stop();
	}
	
	void Update ()
    {
        //Check if enemy is death
        if(enemy.tag == Tags.corpse)
        {
            if (footsteps.isPlaying)
            {
                footsteps.Stop();
                footsteps.Clear();
            }
            return;
        }

        //Check if enemy is visible TODO: if the enemy is stopped don't play footsteps
        if(render.enabled == true)
        {
            if (footsteps.isPlaying)
            {
                footsteps.Stop();
                footsteps.Clear();
            }
        }
        else
        {
            timer += Time.deltaTime;

            if(timer >= time_detection)
            {
                Collider[] players_in_sound_radius = Physics.OverlapSphere(transform.position, sound_radius, player_mask);

                if (players_in_sound_radius.Length > 0)
                {
                    if (footsteps.isPlaying == false)
                        footsteps.Play();
                }
                else
                {
                    footsteps.Stop();
                    footsteps.Clear();
                }
                    

                timer = 0.0f;
            }
          
        }
	
	}
}
