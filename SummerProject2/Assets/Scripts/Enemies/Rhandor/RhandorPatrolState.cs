using UnityEngine;
using System.Collections;

public class RhandorPatrolState : IRhandorStates
{
    private readonly RhandorController rhandor;
    
    // Constructor
    public RhandorPatrolState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public void StartState()
    {
        if (!rhandor.initial_state_assign || !rhandor.same_neutral_alert_path)
        {
            rhandor.current_position = rhandor.findClosestPoint(rhandor.neutral_patrol.path);
            rhandor.initial_state_assign = true;
        }

        rhandor.agent.speed = rhandor.patrol_speed;
        rhandor.agent.SetDestination(rhandor.neutral_patrol.path[rhandor.current_position]);
        rhandor.agent.Resume();

        rhandor.time_waiting_on_position = 0.1f;

        rhandor.movement_allowed = false;
        rhandor.permission_given = false;
        rhandor.waiting_permission = false;
    }

    public void UpdateState()
    {
        // If the alarm is active, the enemy change its current state to Alert
        if (rhandor.alarm_system.isAlarmActive())
            ToAlertState();

        rhandor.CheckNextMovement(rhandor.neutral_patrol);
    }

    public void ToIdleState()
    {
        rhandor.ChangeStateTo(rhandor.idle_state);
    }   

    public void ToAlertState()
    {
        rhandor.ChangeStateTo(rhandor.alert_state);
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

    public void ToPatrolState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state PATROL");
    }
}
