using UnityEngine;
using System.Collections;

public class RhandorIdleState : IRhandorStates
{
    private readonly RhandorController rhandor;
    private bool initial_position_achieved;
    

    public RhandorIdleState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public void StartState()
    {
        rhandor.agent.destination = rhandor.initial_position;
        initial_position_achieved = false;        
    }

    public void UpdateState()
    {
        // Returning to the unique position
        if (!initial_position_achieved)
            ReturningInitialPosition();

        // If the alarm is active, the enemy change its current state to Alert
        if (rhandor.alarm_system.isAlarmActive())
            ToAlertState();
    }

    public void ToIdleState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state IDLE");
    }

    public void ToPatrolState()
    {
        Debug.Log("Enemy" + rhandor.name + "on IDLE, can't change to PATROL state");
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

    private void ReturningInitialPosition()
    {
        if(rhandor.agent.hasPath && rhandor.agent.remainingDistance < rhandor.agent.stoppingDistance)
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
