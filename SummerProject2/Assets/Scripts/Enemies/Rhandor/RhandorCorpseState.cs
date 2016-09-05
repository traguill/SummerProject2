using UnityEngine;
using System.Collections;

public class RhandorCorpseState : IRhandorStates {

    private readonly RhandorController rhandor;

    public RhandorCorpseState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public void StartState()
    {
        rhandor.agent.Stop();
        rhandor.agent.enabled = false;

        // Changes on Tags and Layers
        rhandor.tag = Tags.corpse;
        rhandor.gameObject.layer = LayerMask.NameToLayer("Corpse");

        // Stop looking for players
        rhandor.enemy_field_view.StopCoroutine("FindTargetsWithDelay");  

        // The synchronized Rhandor will not be synchronized anymore
        rhandor.neutral_patrol.synchronized_Rhandor.GetComponent<RhandorController>().neutral_patrol.is_synchronized = false;
        rhandor.neutral_patrol.synchronized_Rhandor.GetComponent<RhandorController>().alert_patrol.is_synchronized = false;
        rhandor.alert_patrol.synchronized_Rhandor.GetComponent<RhandorController>().neutral_patrol.is_synchronized = false;
        rhandor.alert_patrol.synchronized_Rhandor.GetComponent<RhandorController>().alert_patrol.is_synchronized = false;

        //Tint black the enemy if it's dead
        rhandor.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1);
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

    public void ToSupportState()
    {
        Debug.Log("The enemy is dead. Resurrection is still not possible...");
    }

    public void ToCorpseState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state CORPSE");
    }
}
