using UnityEngine;
using System.Collections;

public class EnemyAAlertState : IEnemyAStates
{
    private NavMeshAgent agent;
    private AlarmSystem alarm_system;

    private readonly EnemyAController enemy;

    // Constructor
    public EnemyAAlertState(EnemyAController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public Transform[] AwakeState()
    {
        // For the alert path as an enemy child, we create its corresponding alert_path that 
        // the enemy will use.
        Transform[] alert_patrol;
        if (enemy.alert_path != null)
        {
            alert_patrol = new Transform[enemy.alert_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in enemy.alert_path.transform.GetComponentInChildren<Transform>())
                alert_patrol[i++] = path_unit;
        }
        else
        {
            Debug.Log("There is no alert route for " + enemy.name);
            alert_patrol = new Transform[1];
            alert_patrol[0].position = new Vector3(999.9f, 999.9f, 999.9f);
        }

        agent = enemy.GetComponent<NavMeshAgent>();     // Agent for NavMesh
        //agent.updateRotation = false;                 // Avoiding a 3D rotation of the Sprite
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();

        return alert_patrol;
    }

    public void UpdateState()
    {
        // If the alarm is turned off, the enemy reverts its current state to Patrol
        if (!alarm_system.isAlarmActive())
        {
            ToPatrolState();
        }

        // Choose the next destination point when the agent gets close to the current one.
        if (agent.hasPath && agent.remainingDistance < 0.5f)
            goToNextPoint();
    }

    public void ToIdleState()
    {
        enemy.current_state = enemy.idle_state;
    }

    public void ToPatrolState()
    {
        enemy.current_position = findClosestPoint(enemy.neutral_patrol);
        enemy.current_state = enemy.patrol_state;      
    }

    public void ToAlertState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state ALERT");
    }

    private void goToNextPoint()
    {
        // Returns if no points have been set up
        if (enemy.alert_patrol.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = enemy.alert_patrol[enemy.current_position].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        enemy.current_position = (enemy.current_position + 1) % enemy.alert_patrol.Length;
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
