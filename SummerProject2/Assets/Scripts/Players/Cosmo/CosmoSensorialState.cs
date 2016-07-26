using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CosmoSensorialState : ICosmoState
{
    private readonly CosmoController cosmo;

    bool is_loading = false; //The ability is loading before actually casting.
    float load_time = 0.0f; //Current time loading the ability.
    public float cast_time = 1.5f; //Loading time before casting the ability.

    float detection_radius = 0.0f; //Current detection radius of the ability.
    public float detection_radius_speed = 10; //Speed to increase the detection radius.
    public float max_detection_radius = 20; //Detection range

    public List<Transform> detected_enemies = new List<Transform>(); //Enemies detected by the ability

    LayerMask detection_mask; //Layers that will be detected by the ability.

    public CosmoSensorialState(CosmoController cosmo_controller)
    {
        cosmo = cosmo_controller;
        detection_mask = LayerMask.GetMask("Enemy");
    }

    public void StartState()
    {
        is_loading = false;
        load_time = 0.0f;
        detection_radius = 0.0f;
        detected_enemies.Clear();
    }

    public void UpdateState()
    {
        //Before casting the ability it must load
        if(is_loading)
        {
            load_time += Time.deltaTime;
            if (load_time >= cast_time)
                is_loading = false;
        }
        else //Actual ability effect
        {
            detection_radius += Time.deltaTime * detection_radius_speed;

            if (detection_radius >= max_detection_radius)
            {
                detection_radius = max_detection_radius;
            }

            Collider[] enemies = Physics.OverlapSphere(cosmo.transform.position, detection_radius, detection_mask);

            foreach (Collider enemy in enemies)
            {
                Transform item = enemy.transform;
                if (detected_enemies.Find(x => x.position == item.position) == false)
                {
                    detected_enemies.Add(item); //new enemy detected

                    if (item.gameObject.GetComponent<EnemyController>() != null)
                    {
                        item.gameObject.GetComponent<EnemyController>().SetDetected();
                    }
                }
            }

            if (detection_radius == max_detection_radius) //End ability
            {
                ToIdleState();
            }

        }
    }

    public void ToIdleState()
    {
        cosmo.ChangeStateTo(cosmo.idle_state);
    }

    public void ToSensorialState()
    {
        Debug.Log("Cosmo can't transition from SENSORIAL state to SENSORIAL");
    }

    public void ToWalkingState()
    {
        Debug.Log("Cosmo can't transition from SENSORIAL state to WALKING");
    }

    public void ToHideState()
    {
        Debug.Log("Cosmo can't transition from SENSORIAL state to WALKING");
    }
  
}
