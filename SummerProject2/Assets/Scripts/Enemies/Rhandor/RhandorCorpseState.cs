using UnityEngine;
using System.Collections;

public class RhandorCorpseState : IRhandorStates {

    private readonly RhandorController enemy;

    public RhandorCorpseState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public void StartState()
    {
        
    }

    public void UpdateState()
    {
        enemy.render.flipY = true;
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

    public void ToCorpseState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state CORPSE");
    }
}
