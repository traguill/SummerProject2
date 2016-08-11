using UnityEngine;
using System.Collections;

public class RhandorSpottedState : IRhandorStates
{
    
    private readonly RhandorController enemy;   
    private float time_searching, max_time_searching;

    public RhandorSpottedState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
        max_time_searching = 5.0f;  
    }

    public void StartState()
    {
        time_searching = 0.0f;
        enemy.agent.destination = enemy.last_spotted_position.LastPosition;
        enemy.agent.speed = enemy.spotted_speed;
    }

    public void UpdateState()
    {
        if(enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
        {
            if (time_searching > max_time_searching)
            {
                if (enemy.alarm_system.isAlarmActive())
                    ToAlertState();
                else
                    ToPatrolState();
            }
            else
                time_searching += Time.deltaTime;
        }       
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
        enemy.ChangeStateTo(enemy.alert_state);
    }

    public void ToSpottedState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state SPOTTED");
    }

    public void ToCorpseState()
    {
        enemy.ChangeStateTo(enemy.corpse_state);
    }
}
