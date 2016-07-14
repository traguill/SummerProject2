using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    private GameObject[] players;
    private GameObject[] enemies;
    private Camera cam;

    private Vector3[] initial_player_positions;
    private Vector3[] initial_enemy_positions;
    private Vector3 initial_camera_position;

    private PlayerController[] player_controller;
    private EnemyPatrol[] enemy_patrol_controller;

    // Awake is called before any start function
    void Awake()
    {
        // Saving starting positions for players and getting 
        // player controller for PathFinding related operations
        players = GameObject.FindGameObjectsWithTag(Tags.player);
        initial_player_positions = new Vector3[players.Length];
        player_controller = new PlayerController[players.Length];
        for (int i = 0; i < players.Length; ++i)
        {
            initial_player_positions[i] = players[i].transform.position;
            player_controller[i] = players[i].GetComponent<PlayerController>();
        }

        // Saving starting positions for enemies and getting 
        // enemy patrol controller for PathFinding related operations
        enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        initial_enemy_positions = new Vector3[enemies.Length];
        enemy_patrol_controller = new EnemyPatrol[enemies.Length];
        for (int i = 0; i < enemies.Length; ++i)
        {
            initial_enemy_positions[i] = enemies[i].transform.position;
            enemy_patrol_controller[i] = enemies[i].GetComponent<EnemyPatrol>();
        }

        // Saving starting positions for camera
        cam = Camera.main;
        initial_camera_position = cam.transform.position;
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.A))
            resetGame();           
	}

    void resetGame()
    {
        // Reseting player positions and finishing pathfinding activity
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].transform.position = initial_player_positions[i];
            player_controller[i].stopMovement();
        }

        // Reseting enemy positions and finishing pathfinding activity
        for (int i = 0; i < enemies.Length; ++i)
        {
            enemies[i].transform.position = initial_enemy_positions[i];
            enemy_patrol_controller[i].stopMovement();
            enemy_patrol_controller[i].initiateMovement();
        }

        // Resetting camera
        cam.transform.position = initial_camera_position;
    }

}
