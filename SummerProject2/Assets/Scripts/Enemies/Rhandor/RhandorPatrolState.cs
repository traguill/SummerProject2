using UnityEngine;
using System.Collections;

public class RhandorPatrolState : IRhandorStates
{
    private readonly RhandorController enemy;
    
    // Constructor
    public RhandorPatrolState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
    }

    public Transform[] AwakeState()
    {
        // For the neutral path as an enemy child, we create its corresponding patrol_path that 
        // the enemy will use.
        Transform[] neutral_patrol;
        if (enemy.neutral_path != null)
        {
            neutral_patrol = new Transform[enemy.neutral_path.transform.childCount];

            int i = 0;
            foreach (Transform path_unit in enemy.neutral_path.transform.GetComponentInChildren<Transform>())
                neutral_patrol[i++] = path_unit;           
        }            
        else
        {
            Debug.Log("There is no patrol route for " + enemy.name);
            neutral_patrol = new Transform[1];
            neutral_patrol[0].position = new Vector3(999.9f, 999.9f, 999.9f);
        }
          
        //enemy.current_position = 0;
        enemy.agent = enemy.GetComponent<NavMeshAgent>();     // Agent for NavMesh  
        
        return neutral_patrol;
    }

    public void StartState()
    {
        enemy.agent.speed = enemy.patrol_speed;
      
        enemy.current_position = enemy.findClosestPoint(enemy.neutral_patrol);
        enemy.agent.destination = enemy.neutral_patrol[enemy.current_position].position;

        enemy.time_waiting_on_position = 0.1f;
    }

    public void UpdateState()
    {
        // If the alarm is active, the enemy change its current state to Alert
        if (enemy.alarm_system.isAlarmActive())
        {
            // If the alarm is active, the enemy goes to that point to find the player.
            if (enemy.last_spotted_position.IsPlayerSpotted())
                ToSpottedState();
            else
                ToAlertState();
        }

        enemy.CheckNextMovement(enemy.neutral_patrol, enemy.stopping_time_neutral_patrol);

    }

    public void ToIdleState()
    {
        enemy.ChangeStateTo(enemy.idle_state);
    }

    public void ToPatrolState()
    {
        Debug.Log("Enemy" + enemy.name + "can't transition to same state PATROL");
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
}
