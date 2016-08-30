using UnityEngine;
using System.Collections;

public class RhandorPatrolState : IRhandorStates
{
    private readonly RhandorController rhandor;
    
    // Constructor
    public RhandorPatrolState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public Transform[] AwakeState()
    {
        // For the neutral path gameobject attach to this enemy, we create its corresponding patrol_path that 
        // the enemy will use, considering or not a loop patrol.

        Transform[] neutral_patrol;
        if (rhandor.neutral_path != null)
        {
            neutral_patrol = new Transform[rhandor.neutral_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in rhandor.neutral_path.transform.GetComponentInChildren<Transform>())
                neutral_patrol[i++] = path_unit;                       
        }            
        else
        {
            neutral_patrol = null;
        }
          
        rhandor.agent = rhandor.GetComponent<NavMeshAgent>();     // Agent for NavMesh  
        
        return neutral_patrol;
    }

    public void StartState()
    {
        rhandor.agent.speed = rhandor.patrol_speed;
      
        rhandor.current_position = rhandor.findClosestPoint(rhandor.neutral_patrol);
        rhandor.agent.destination = rhandor.neutral_patrol[rhandor.current_position].position;
        rhandor.agent.Resume();

        rhandor.time_waiting_on_position = 0.1f;
    }

    public void UpdateState()
    {
        // If the alarm is active, the enemy change its current state to Alert
        if (rhandor.alarm_system.isAlarmActive())
        {
            ToAlertState();
            //// If the alarm is active, the enemy goes to that point to find whatever it is.
            //if (rhandor.last_spotted_position.IsSomethingSpotted())
            //    ToSpottedState();
            //else
            //    ToAlertState();
        }

        rhandor.CheckNextMovement(rhandor.neutral_patrol, rhandor.stopping_time_neutral_patrol, rhandor.neutral_path_loop);
    }

    public void ToIdleState()
    {
        rhandor.ChangeStateTo(rhandor.idle_state);
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

    public void ToPatrolState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state PATROL");
    }
}
