using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SensorialAbility : MonoBehaviour
{
    public float cast_time = 1.5f; //Delay time to load the ability before actually cast it.
    float load_time = 0.0f;

    [HideInInspector]
    public bool is_playing = false;
    bool is_loading = false;

    public float max_detection_radius;
    [HideInInspector]
    public float detection_radius = 0.0f;
    public LayerMask detection_mask;
    public float detection_radius_speed; //Speed to increase the radius of detection.

    [HideInInspector]
    public List<Transform> detected_enemies;

    void Update()
    {
        if(is_playing)
        {
            if(is_loading)
            {
                load_time += Time.deltaTime;
                if (load_time >= cast_time)
                    is_loading = false;
            }
            else //actual effect of the ability
            {
                detection_radius += Time.deltaTime * detection_radius_speed;

                if (detection_radius >= max_detection_radius) 
                {
                    detection_radius = max_detection_radius;
                }

                Collider[] enemies = Physics.OverlapSphere(transform.position, detection_radius, detection_mask);

                foreach(Collider enemy in enemies)
                {
                    Transform item = enemy.transform;
                    if(detected_enemies.Find(x =>x.position == item.position) == false)
                    {
                        detected_enemies.Add(item); //new enemy detected
                        
                        if(item.gameObject.GetComponent<EnemyController>() != null)
                        {
                            item.gameObject.GetComponent<EnemyController>().SetDetected();
                        }
                    }
                }

                if(detection_radius == max_detection_radius) //End ability
                {
                    is_playing = false;
                    detection_radius = 0.0f;
                    detected_enemies.Clear();
                }

            }
        }
    }


    public void UseAbility()
    {
        if(is_playing == false)
        {
            is_playing = true;
            is_loading = true;
        }
    }
}
