using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyVisibility : MonoBehaviour
{
    //Detection
    private bool is_visible;
    private bool is_detected;
    private float detected_time;
    private MeshRenderer render;
    public Material detected_material;
    public Material normal_material;
    public float max_detection_time = 3.0f;

    private EnemyManager enemy_manager; // EnemyManager
    MeshRenderer arrow;                 // DEBUG --> Arrow

    void Awake()
    {
        render = GetComponent<MeshRenderer>();
        render.enabled = false;                     // Set to invisible by default
        is_detected = false;
        is_visible = false;
        detected_time = 0.0f;

        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        arrow = transform.FindChild("Arrow3D").gameObject.GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        SetVisible();
        Visualization();
	}

    private void Visualization()
    {
        if (is_visible || enemy_manager.fow_disabled)
        {
            render.enabled = true;      // Visible
            arrow.enabled = true;
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
                arrow.enabled = false;
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
            foreach (Transform spotted_entity in player.GetComponent<FieldOfView>().visible_targets)
            {
                if(spotted_entity == transform)
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

            render.enabled = true;
            arrow.enabled = true;
        }
    } 
   
    /// <summary>
    /// Is the enemy visible at any way for the player.
    /// </summary>
    /// <returns></returns>
    public bool IsVisible()
    {
        bool ret = false;

        if (is_detected || is_visible)
            ret = true;

        return ret;
    }
}
