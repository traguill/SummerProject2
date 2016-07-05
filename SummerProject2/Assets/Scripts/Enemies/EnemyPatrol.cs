using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

    private Transform[] patrol_path;
    private int current_position;
    private NavMeshAgent agent;

    void Awake()
    {
        GameObject waypoints = GameObject.Find("Patrol_Routes");
  
        patrol_path = new Transform[GameObject.FindGameObjectsWithTag("PatrolUnitPath").Length];

        int i = patrol_path.Length - 1;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("PatrolUnitPath"))
        {
            patrol_path[i] = g.GetComponent<Transform>();
            i--;
        }
        
        agent = GetComponent<NavMeshAgent>();
    }

    // Use this for initialization
    void Start()
    {
       goToNextPoint();
    }

    // Update is called once per frame
    void Update ()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (agent.remainingDistance < 0.5f)
            goToNextPoint();
    }

    void goToNextPoint()
    {
        // Returns if no points have been set up
        if (patrol_path.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = patrol_path[current_position].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        current_position = (current_position + 1) % patrol_path.Length;
    }
}
