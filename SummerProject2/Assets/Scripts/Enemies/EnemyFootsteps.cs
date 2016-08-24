using UnityEngine;
using System.Collections;

public class EnemyFootsteps : MonoBehaviour
{
    GameObject enemy;
    ParticleSystem footsteps;

    LayerMask player_mask;

    public float sound_radius;

	void Start ()
    {
        player_mask = LayerMask.GetMask("Player");
        enemy = transform.parent.gameObject;
        footsteps = GetComponent<ParticleSystem>();

        footsteps.Stop();
	}
	
	void Update ()
    {
        //Check if enemy is death
        if(enemy.tag == Tags.corpse)
        {
            if (footsteps.isPlaying)
                footsteps.Stop();

            return;
        }

        //Check if enemy is visible TODO: change this implementation
        if(enemy.GetComponent<MeshRenderer>().enabled == true)
        {
            if (footsteps.isPlaying)
                footsteps.Stop();
        }
        else
        {
            Collider[] players_in_sound_radius = Physics.OverlapSphere(transform.position, sound_radius, player_mask);

            if(players_in_sound_radius.Length > 0)
                if (footsteps.isPlaying == false)
                     footsteps.Play();
        }
	
	}
}
