using UnityEngine;
using System.Collections;

public class LastSpottedPosition : MonoBehaviour {

    public Vector3 last_position;

	// Use this for initialization
	void Start ()
    {
        last_position = new Vector3(0, -1, 0);
	}
	
	// Update is called once per frame
	void Update ()
    { }

    public void SetLastSpottedPosition(Vector3 new_position)
    {
        last_position = new_position;
    }
}
