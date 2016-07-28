using UnityEngine;
using System.Collections;

public class EnemyAIdleState : IEnemyAStates
{
    private readonly EnemyAController enemy;

    public EnemyAIdleState(EnemyAController enemy_controller)
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

}
