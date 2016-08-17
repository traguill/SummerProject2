﻿using UnityEngine;
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
    }

    public void UpdateState()
    {
        enemy.render.flipY = false; //Due to the shader the sprite is already flipped.
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
