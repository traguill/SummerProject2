using UnityEngine;
using System.Collections;

public class RhandorIdleState : IRhandorStates
{
    private readonly RhandorController enemy;
    private bool initial_position_achieved;

    public RhandorIdleState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public void StartState()
    {
        enemy.agent.destination = enemy.initial_position;
        initial_position_achieved = false;
    }

    public void UpdateState()
    {
        // Returning to the unique position
        if (!initial_position_achieved)
            ReturningInitialPosition();

        // If the alarm is active, the enemy change its current state to Alert
        if (enemy.alarm_system.isAlarmActive())
        {
            // If the alarm is active, the enemy goes to that point to find the player.
            if (enemy.last_spotted_position.IsPlayerSpotted())
                ToSpottedState();
            else
                ToAlertState();
        }
    }

    public void ToIdleState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state IDLE");
    }

    public void ToPatrolState()
    {
        Debug.Log("Enemy" + enemy.name + "on IDLE, can't change to PATROL state");
    }

    public void ToAlertState()
    {
        enemy.ChangeStateTo(enemy.alert_state);
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
        if(enemy.agent.hasPath && enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
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
