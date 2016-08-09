﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyVisibility : MonoBehaviour
{
    //Detection
    private bool is_visible;
    private bool is_detected;
    private float detected_time;
    private SpriteRenderer render;
    public Material detected_material;
    public Material normal_material;
    public float max_detection_time = 3.0f;

    // EnemyManager
    private EnemyManager enemy_manager;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        render.enabled = false; //Set to invisible by default
        is_detected = false;
        is_visible = false;
        detected_time = 0.0f;

        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        SetVisible();
        Visualization();
	}

    private void Visualization()
    {
        if (is_visible)
        {
            render.enabled = true; // Visible
            if (is_detected)
            {
                is_detected = false;
                render.material = normal_material;
            }
        }
        else
        {
            if (is_detected)
            {
                render.enabled = true;
                detected_time += Time.deltaTime;

                if (detected_time > max_detection_time)
                {
                    is_detected = false;
                    render.material = normal_material;
                }
            }
            else
            {
                render.enabled = false; //Inside the fow
            }
        }         
    }

    /// <summary>
    /// Sets the enemies visible or not depending of the fog of war.
    /// </summary>
    private void SetVisible()
    {
        is_visible = false;        

        // Then, the spotted enemy turn to visible if some player is seeing him.
        foreach (GameObject player in enemy_manager.players)
        {
            foreach (Transform spotted_enemy in player.GetComponent<FieldOfView>().visible_targets)
            {
                if(spotted_enemy == transform)
                {
                    is_visible = true;
                }
            }
        }
    }

    /// <summary>
    /// Tells to the enemy that is being detected by the sensorial ability.
    /// </summary>
    public void SetDetected()
    {
        if(is_detected == false)
        {
            detected_time = 0.0f;
            is_detected = true;

            //Show new visualization
            render.material = detected_material;
        }
    }    
}