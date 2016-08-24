using UnityEngine;
using System.Collections;

public class RhandorAlertState : IRhandorStates
{   
    private readonly RhandorController enemy;
    private bool initial_position_achieved;

    // Constructor
    public RhandorAlertState(RhandorController enemy_controller)
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
            alert_patrol = null;
        }

        return alert_patrol;
    }

    public void StartState()
    {
        if(!enemy.static_alert)
        {
            enemy.agent.speed = enemy.alert_speed;
            enemy.current_position = enemy.findClosestPoint(enemy.alert_patrol);
            enemy.agent.destination = enemy.alert_patrol[enemy.current_position].position;

            enemy.time_waiting_on_position = 0.1f;
        }  
        else
        {
            enemy.agent.destination = enemy.initial_position;
            initial_position_achieved = false;
        }     
    }

    public void UpdateState()
    {
        // Returning to the unique position
        if (!initial_position_achieved)
            ReturningInitialPosition();

        // If the alarm is turned off, the enemy reverts its current state to Patrol
        if (!enemy.alarm_system.isAlarmActive())
        {
            if (enemy.static_neutral)
                ToIdleState();
            else
                ToPatrolState();
        }

        if(!enemy.static_alert)
            enemy.CheckNextMovement(enemy.alert_patrol, enemy.stopping_time_alert_patrol, enemy.alert_path_loop);
    }

    public void ToIdleState()
    {
        enemy.ChangeStateTo(enemy.idle_state);
    }

    public void ToPatrolState()
    {
        enemy.ChangeStateTo(enemy.patrol_state);
    }

    public void ToAlertState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state ALERT");
    }

    public void ToSpottedState()
    {
        enemy.ChangeStateTo(enemy.spotted_state);
    }

    public void ToCorpseState()
    {
        enemy.ChangeStateTo(enemy.corpse_state);
    }

    private void ReturningInitialPosition()
    {
        if (enemy.agent.hasPath && enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
        {
            enemy.agent.ResetPath();
            enemy.agent.Stop();
        }

        if (!enemy.agent.hasPath && !initial_position_achieved)
        {
            if (enemy.transform.forward.IsCloseTo(enemy.initial_forward_direction, 0.01f))
                initial_position_achieved = true;
            else
                enemy.RotateTowards(enemy.initial_forward_direction);
        }
    }
}
