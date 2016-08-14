using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RhandorController : Enemies {

    // NavMeshAgent variables and patrol routes
    public GameObject neutral_path, alert_path;
    public float patrol_speed, alert_speed, spotted_speed;
    [HideInInspector] public float[] stopping_time_neutral_patrol = new float[1];
    [HideInInspector] public float[] stopping_time_alert_patrol = new float[1];
    [HideInInspector] public int num_neutral_waypoints = 0, num_alert_waypoints = 0;
    [HideInInspector] public float time_waiting_on_position;
    [HideInInspector] public NavMeshAgent agent;   
    [HideInInspector] public Transform[] neutral_patrol, alert_patrol;    
    [HideInInspector] public int current_position;

    // For corpses representation
    [HideInInspector] public SpriteRenderer render;

    //State machine
    [HideInInspector] IRhandorStates current_state;
    [HideInInspector] public RhandorIdleState idle_state;
    [HideInInspector] public RhandorPatrolState patrol_state;
    [HideInInspector] public RhandorAlertState alert_state;
    [HideInInspector] public RhandorSpottedState spotted_state;
    [HideInInspector] public RhandorCorpseState corpse_state;

    // Scripts references
    [HideInInspector] public AlarmSystem alarm_system;
    [HideInInspector] public LastSpottedPosition last_spotted_position;

    void Awake()
    {
        // State machine
        // -- IDLE --
        idle_state = new RhandorIdleState(this);
        // -- PATROL --
        patrol_state = new RhandorPatrolState(this);
        neutral_patrol = patrol_state.AwakeState();  // It transforms the path GameObject to Transform[]
        // -- ALERT --
        alert_state = new RhandorAlertState(this);
        alert_patrol = alert_state.AwakeState();     // It transforms the path GameObject to Transform[] 
        // -- SPOTTED --
        spotted_state = new RhandorSpottedState(this);
        // -- CORPSE --
        corpse_state = new RhandorCorpseState(this);

        render = GetComponent<SpriteRenderer>();
        type = ENEMY_TYPES.RHANDOR;

        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        last_spotted_position = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<LastSpottedPosition>();
    }
    
    void Start()
    {
        ChangeStateTo(patrol_state);
    }

    void Update()
    {
        current_state.UpdateState();
    }

    public void ChangeStateTo(IRhandorStates new_state)
    {
        current_state = new_state;
        current_state.StartState();
    }

    public void Dead()
    {
        if (current_state != corpse_state)
            ChangeStateTo(corpse_state);
        else
            Debug.Log("The enemy is already dead!");
    }

    /// <summary>
    /// Returns the current state.
    /// </summary>
    public IRhandorStates GetState()
    {
        return current_state;
    }

    /// <summary>
    /// goToNextPoint gives the index for the next position of the argument current path
    /// </summary>
    /// <returns> The index of the next position </returns>
    public void goToNextPoint(Transform[] current_path)
    {
        // Returns if no points have been set up
        if (current_path.Length == 0)
            return;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        current_position = (current_position + 1) % current_path.Length;

        // Set the agent to go to the currently selected destination.
        agent.destination = current_path[current_position].position;        
    }

    /// <summary>
    /// findClosestPoint finds the nearest position from the current patrol path
    /// assigned to this enemy. It is used when the game switches between patroling states.
    /// </summary>
    /// <returns> The index of the closest position </returns>
    public int findClosestPoint(Transform[] path_to_search)
    {
        int index = -1;
        NavMeshPath path = new NavMeshPath();
        float minimum_distance = 1000000.0f;

        for (int i = 0; i < path_to_search.Length; ++i)
        {
            agent.CalculatePath(path_to_search[i].position, path);
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

    /// <summary>
    ///  Check if the enemy has to change its destination or wait the number of seconds the user
    ///  has introduced.
    /// </summary>
    public void CheckNextMovement(Transform[] current_path, float[] current_stopping_times)
    {
        //Choose the next destination point when the agent gets close to the current one.
        // I have eliminated this snippet from the IF --> enemy.agent.hasPath &&
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            if (time_waiting_on_position > current_stopping_times[current_position])
            {
                goToNextPoint(current_path);
                time_waiting_on_position = 0.1f;
                agent.Resume();
            }
            else
            {
                time_waiting_on_position += Time.deltaTime;
                agent.Stop();
                agent.ResetPath();
            }
        }
    }
}

