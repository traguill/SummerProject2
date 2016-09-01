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
        if(rhandor.alert_patrol.static_patrol)
        {
            rhandor.agent.SetDestination(rhandor.initial_position);
            initial_position_achieved = false;            
        }  
        else
        {
            rhandor.agent.speed = rhandor.alert_speed;
            rhandor.current_position = rhandor.findClosestPoint(rhandor.alert_patrol.path);
            rhandor.agent.SetDestination(rhandor.alert_patrol.path[rhandor.current_position]);
            rhandor.agent.Resume();

            rhandor.time_waiting_on_position = 0.1f;
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
            if (rhandor.neutral_patrol.static_patrol)
                ToIdleState();
            else
                ToPatrolState();
        }

        if(!rhandor.alert_patrol.static_patrol)
            rhandor.CheckNextMovement(rhandor.alert_patrol.path, rhandor.alert_patrol.stop_times, rhandor.alert_patrol.loop);
    }

    public void ToIdleState()
    {
        rhandor.ChangeStateTo(rhandor.idle_state);
    }

    public void ToPatrolState()
    {
        rhandor.ChangeStateTo(rhandor.patrol_state);
    }

    public void ToSpottedState()
    {
        rhandor.ChangeStateTo(rhandor.spotted_state);
    }

    public void ToSupportState()
    {
        rhandor.ChangeStateTo(rhandor.support_state);
    }

    public void ToCorpseState()
    {
        rhandor.ChangeStateTo(rhandor.corpse_state);
    }

    public void ToAlertState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state ALERT");
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
