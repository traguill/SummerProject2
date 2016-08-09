using UnityEngine;
using System.Collections;

public class RhandorPatrolState : IRhandorStates
{
    private NavMeshAgent agent;
    private float patrol_speed;
    private AlarmSystem alarm_system;

    private readonly RhandorController enemy;

    // Constructor
    public RhandorPatrolState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public Transform[] AwakeState()
    {
        // For the neutral path as an enemy child, we create its corresponding patrol_path that 
        // the enemy will use.
        Transform[] neutral_patrol;
        if (enemy.neutral_path != null)
        {
            neutral_patrol = new Transform[enemy.neutral_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in enemy.neutral_path.transform.GetComponentInChildren<Transform>())
                neutral_patrol[i++] = path_unit;           
        }            
        else
        {
            Debug.Log("There is no patrol route for " + enemy.name);
            neutral_patrol = new Transform[1];
            neutral_patrol[0].position = new Vector3(999.9f, 999.9f, 999.9f);
        }
          
        enemy.current_position = 0;
        agent = enemy.GetComponent<NavMeshAgent>();     // Agent for NavMesh
        patrol_speed = 2.5f;
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();

        return neutral_patrol;
    }

    public void StartState()
    {
        agent.speed = patrol_speed;
        enemy.current_position = findClosestPoint(enemy.neutral_patrol);
        goToNextPoint();
    }

    public void UpdateState()
    {
        enemy.ChangeStateTo(enemy.corpse_state);

        // If the alarm is active, the enemy change its current state to Alert
        if (alarm_system.isAlarmActive())
        {
            ToAlertState();
        }             

        // Choose the next destination point when the agent gets close to the current one.
        if (agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
            goToNextPoint();
    }

    public void ToIdleState()
    {
        enemy.ChangeStateTo(enemy.idle_state);
    }

    public void ToPatrolState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state PATROL");
    }

    public void ToAlertState()
    {
        enemy.ChangeStateTo(enemy.alert_state);
    }

    public void ToCorpseState()
    {
        enemy.ChangeStateTo(enemy.corpse_state);
    }

    private void goToNextPoint()
    {
        // Returns if no points have been set up
        if (enemy.neutral_patrol.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = enemy.neutral_patrol[enemy.current_position].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        enemy.current_position = (enemy.current_position + 1) % enemy.neutral_patrol.Length;
    }

    /// <summary>
    /// findClosestPoint finds the nearest position from the current patrol path
    /// assigned to this enemy. It is used when the game switches between alarm states.
    /// </summary>
    /// <returns> The index of the closest position </returns>
    private int findClosestPoint(Transform[] path_to_search)
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

}
