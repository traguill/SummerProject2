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
        enemy.agent.Stop();
        enemy.agent.enabled = false;
        enemy.enemy_field_view.StopCoroutine("FindTargetsWithDelay");

        //Tint black the enemy if it's dead
        enemy.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1);
    }

    public void UpdateState()
    {
        
    }

    public void ToIdleState()
    {
        Debug.Log("The enemy is dead. Resurrection is still not possible...");
    }

    public void ToPatrolState()
    {
        Debug.Log("The enemy is dead. Resurrection is still not possible...");
    }

    public void ToAlertState()
    {
        Debug.Log("The enemy is dead. Resurrection is still not possible...");
    }

    public void ToSpottedState()
    {
        Debug.Log("The enemy is dead. Resurrection is still not possible...");
    }

    public void ToCorpseState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state CORPSE");
    }
}
