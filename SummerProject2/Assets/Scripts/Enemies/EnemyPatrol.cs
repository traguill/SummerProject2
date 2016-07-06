using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

    private Transform[] patrol_path;
    private int current_position;
    private NavMeshAgent agent;

    void Awake()
    {
        GameObject path = GameObject.Find("Patrol_path_" + name);
        if(path != null)
        {
            // linking its corresponding patrol route.
            patrol_path = new Transform[path.transform.childCount];
            int i = 0;
            foreach (Transform path_unit in path.transform.getChilds())
            {
                patrol_path[i] = path_unit;
                ++i;
            }
        }
        else
        {
            Debug.Log("There is no patrol route for " + name);
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
