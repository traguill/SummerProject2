using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {   

    public GameObject neutral_path, alarm_path;
    private Transform[] neutral_patrol, alarm_patrol, current_patrol;
    private int current_position;
    private NavMeshAgent agent;
    private ALARM_STATE alarm_state;
    private EnemyFieldView enemy_field_view;
   
    void Awake()
    {
        // For the neutral path assigned on inspector, we create its corresponding patrol_path that 
        // the enemy will use.
        if(neutral_path != null)
        {
            neutral_patrol = new Transform[neutral_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in neutral_path.transform.GetComponentInChildren<Transform>())
                neutral_patrol[i++] = path_unit;
        }
        else
            Debug.Log("There is no patrol route for " + name);

        // For the alarm path assigned on inspector, we create its corresponding alarm_patrol_path that 
        // the enemy will use when the alarm is activated.
        if (alarm_path != null)
        {
            alarm_patrol = new Transform[alarm_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in alarm_path.transform.GetComponentInChildren<Transform>())
                alarm_patrol[i++] = path_unit;
        }
        else
            Debug.Log("There is no alarm patrol route for " + name);

        current_patrol = neutral_patrol;

        agent = GetComponent<NavMeshAgent>();   // Agent for NavMesh
        enemy_field_view = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyFieldView>();
        alarm_state = enemy_field_view.getAlarmState();
    }

    // Use this for initialization
    void Start()
    {
       goToNextPoint();
    }

    // Update is called once per frame
    void Update ()
    {
        // Every time the alarm changes, the enemy will find the closest point
        // to continue its patrol.

        if (alarm_state != enemy_field_view.getAlarmState())
        {
            alarm_state = enemy_field_view.getAlarmState();
            switch (alarm_state)
            {
                case (ALARM_STATE.ALARM_OFF):
                    {
                        current_patrol = neutral_patrol;
                        break;
                    }
                case (ALARM_STATE.ALARM_ON):
                    {
                        current_patrol = alarm_patrol;
                        break;
                    }
            }

            current_position = findClosestPoint();
        }       

        // Choose the next destination point when the agent gets
        // close to the current one.
        if (agent.hasPath && agent.remainingDistance < 0.5f)
            goToNextPoint();
    }

    void goToNextPoint()
    {
        // Returns if no points have been set up
        if (current_patrol.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = current_patrol[current_position].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        current_position = (current_position + 1) % current_patrol.Length;
    }

    int findClosestPoint()
    {
        int index = -1;
        NavMeshPath path = new NavMeshPath();
        float minimum_distance = 1000000.0f;

        for (int i = 0; i < current_patrol.Length; ++i)
        {
            agent.CalculatePath(current_patrol[i].position, path);
            float distance = 0;
            for (int j = 0; j < path.corners.Length - 1; ++j)
            {
                distance += Vector3.Distance(path.corners[j], path.corners[j + 1]);
            }
            
            if (distance < minimum_distance)
            {
                index = i;
                minimum_distance = distance;
            }
        }

        return index;
    }
}
