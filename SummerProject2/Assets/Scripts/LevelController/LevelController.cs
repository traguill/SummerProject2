using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{

    private Vector3[] players_positions;
    private Vector3[] enemies_positions;
    private Vector3 camera_position;
    

    // Awake
    void Awake()
    {
        // Saving starting positions for players
        GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.player);
        for (int i = 0; i < players.Length; ++i)
            players_positions[i] = players[i].transform.position;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        for (int i = 0; i < players.Length; ++i)
            players_positions[i] = players[i].transform.position;

    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
