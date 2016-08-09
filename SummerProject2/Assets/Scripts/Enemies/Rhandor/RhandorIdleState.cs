using UnityEngine;
using System.Collections;

public class RhandorIdleState : IRhandorStates
{
    private readonly RhandorController enemy;

    public RhandorIdleState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public void StartState()
    {
       // Enemies don't have IDLE! For now...
    }

    public void UpdateState()
    {
        // Enemy doesn't have a IDLE state right now.
    }

    public void ToIdleState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state IDLE");
    }

    public void ToPatrolState()
    {
        enemy.ChangeStateTo(enemy.patrol_state);
    }

    public void ToAlertState()
    {
        enemy.ChangeStateTo(enemy.alert_state);
    }

    public void ToCorpseState()
    {
        enemy.ChangeStateTo(enemy.corpse_state);
    }

}
