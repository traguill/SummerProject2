using UnityEngine;
using System.Collections;

public class LastSpottedPosition : MonoBehaviour {

    public Vector3 last_position;
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

    public void SetLastSpottedPosition(Vector3 new_position)
    {
        last_position = new_position;
        player_spotted = true;
    }

    public bool IsPlayerSpotted()
    {
        return player_spotted;
    }
}
