using UnityEngine;
using System.Collections;

public class LastSpottedPosition : MonoBehaviour {

    private Vector3 last_position;
    private bool player_spotted;

	// Use this for initialization
	void Start ()
    {
        last_position = new Vector3(0, 0, 0);
        player_spotted = false;
	}
	
	// Update is called once per frame
	void Update ()
    { }

    // Property
    public Vector3 LastPosition
    {
        set
        {
            last_position = value;
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
