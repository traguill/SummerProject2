using UnityEngine;
using System.Collections;

public class RhandorAlertState : IRhandorStates
{   
    private readonly RhandorController rhandor;
    private bool initial_position_achieved;

    // Constructor
    public RhandorAlertState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public Transform[] AwakeState()
    {
        // For the alert path as an enemy child, we create its corresponding alert_path that 
        // the enemy will use.
        Transform[] alert_patrol;
        if (rhandor.alert_path != null)
        {
            alert_patrol = new Transform[rhandor.alert_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in rhandor.alert_path.transform.GetComponentInChildren<Transform>())
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
        if(!rhandor.static_alert)
        {
            rhandor.agent.speed = rhandor.alert_speed;
            rhandor.current_position = rhandor.findClosestPoint(rhandor.alert_patrol);
            rhandor.agent.destination = rhandor.alert_patrol[rhandor.current_position].position;

            rhandor.time_waiting_on_position = 0.1f;
        }  
        else
        {
            rhandor.agent.destination = rhandor.initial_position;
            initial_position_achieved = false;
        }     
    }

    public void UpdateState()
    {
        // Returning to the unique position
        if (!initial_position_achieved)
            ReturningInitialPosition();

        // If the alarm is turned off, the enemy reverts its current state to Patrol
        if (!rhandor.alarm_system.isAlarmActive())
        {
            if (rhandor.static_neutral)
                ToIdleState();
            else
                ToPatrolState();
        }

        if(!rhandor.static_alert)
            rhandor.CheckNextMovement(rhandor.alert_patrol, rhandor.stopping_time_alert_patrol, rhandor.alert_path_loop);
    }

    public void ToIdleState()
    {
        rhandor.ChangeStateTo(rhandor.idle_state);
    }

    public void ToPatrolState()
    {
        rhandor.ChangeStateTo(rhandor.patrol_state);
    }

    public void ToAlertState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state ALERT");
    }

    public void ToSpottedState()
    {
        rhandor.ChangeStateTo(rhandor.spotted_state);
    }

    public void ToCorpseState()
    {
        rhandor.ChangeStateTo(rhandor.corpse_state);
    }

    private void ReturningInitialPosition()
    {
        if (rhandor.agent.hasPath && rhandor.agent.remainingDistance < rhandor.agent.stoppingDistance)
        {
            rhandor.agent.ResetPath();
            rhandor.agent.Stop();
        }

        if (!rhandor.agent.hasPath && !initial_position_achieved)
        {
            initial_position_achieved = rhandor.RecoveringInitialPosition();
        }
    }
}
