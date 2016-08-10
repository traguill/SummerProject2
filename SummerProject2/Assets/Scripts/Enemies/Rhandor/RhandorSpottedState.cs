using UnityEngine;
using System.Collections;

public class RhandorSpottedState : IRhandorStates
{
    private readonly RhandorController enemy;

    public RhandorSpottedState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public void StartState()
    {
        
    }

    public void UpdateState()
    {
        
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
