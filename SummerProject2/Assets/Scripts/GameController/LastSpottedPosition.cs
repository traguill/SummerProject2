using UnityEngine;
using System.Collections;

public class LastSpottedPosition : MonoBehaviour {

    private Vector3 last_position;
    private bool player_spotted;
    private float time_for_new_last_position, max_time;

	// Use this for initialization
	void Start ()
    {
        last_position = new Vector3(0, 0, 0);
        time_for_new_last_position = 0.0f;
        max_time = 5.0f;
        player_spotted = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(player_spotted)
        {
            if (time_for_new_last_position > max_time)
            {
                player_spotted = false;
                time_for_new_last_position = 0.0f;
            }
            else
            {
                time_for_new_last_position += Time.deltaTime;
            }
        }            
    }

    // Property
    public Vector3 LastPosition
    {
        set
        {
            last_position = value;
            time_for_new_last_position = 0.0f;
            player_spotted = true;
        }

        get
        {
            return last_position;
        }
    }

    public bool IsPlayerSpotted()
    {
        return player_spotted;
    }
}
