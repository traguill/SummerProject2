using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public enum State
    {
        IDLE,
        CORPSE
    };

    State state;

    [HideInInspector]
    public bool is_visible = false;

    //Detection
    bool is_detected = false;
    public float max_detection_time = 3.0f;
    float detected_time = 0.0f;
    public Material detected_material;
    public Material normal_material;

    SpriteRenderer render;

    void Start ()
    {
        render = GetComponent<SpriteRenderer>();
        render.enabled = false; //Set to invisible by default
        state = State.IDLE;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Visualization();
	}

    private void Visualization()
    {
        if (is_visible)
        {
            render.enabled = true; //Visible
            if(is_detected)
            {
                is_detected = false;
                render.material = normal_material;
            }
        }
        else
        {
            if(is_detected)
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

    public void ChangeState(State new_state)
    {
        state = new_state;
    }
}
