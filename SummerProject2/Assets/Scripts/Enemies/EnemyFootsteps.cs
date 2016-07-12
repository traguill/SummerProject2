using UnityEngine;
using System.Collections;

public class EnemyFootsteps : MonoBehaviour
{
    GameObject enemy;
    ParticleSystem footsteps;

	void Start ()
    {
        enemy = transform.parent.gameObject;
        footsteps = GetComponent<ParticleSystem>();

        footsteps.Stop();
	}
	
	void Update ()
    {
        //Check if enemy is visible
        if(enemy.GetComponent<SpriteRenderer>().enabled == true)
        {
            if (footsteps.isPlaying)
                footsteps.Stop();
        }
        else
        {
            if (footsteps.isPlaying == false)
                footsteps.Play();
        }
	
	}
}
