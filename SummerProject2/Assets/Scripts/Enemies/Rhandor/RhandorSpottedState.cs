using UnityEngine;
using System.Collections;

public class RhandorSpottedState : IRhandorStates
{
    
    private readonly RhandorController enemy;   
    private float time_searching, max_time_searching;
    private bool corpse_identification = false;

    public RhandorSpottedState(RhandorController enemy_controller)
    {
        enemy = enemy_controller;
        max_time_searching = 2.5f;  
    }

    public void StartState()
    {
        time_searching = 0.0f;
        corpse_identification = false;
        enemy.agent.destination = enemy.last_spotted_position.LastPosition;
        enemy.agent.speed = enemy.spotted_speed;
    }

    public void UpdateState()
    {
        // Rhandor checks distance to recalculate the final destination, to not stop over the corpse.
        // The alarm could be activated in the same moment that is approaching.
        if (!corpse_identification)
        {
            CorpseSpotted();
        }

        if(enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
        {
            if (time_searching > max_time_searching)
            {
                if (enemy.alarm_system.isAlarmActive())
                    ToAlertState();
                else
                {
                    if (enemy.static_neutral)
                        ToIdleState();
                    else
                        ToPatrolState();
                }                    
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

    private void CorpseSpotted()
    {
        float min_dist = 8.0f;
        if(Vector3.Distance(enemy.agent.destination, enemy.transform.position) < min_dist)
        {
            Collider[] targets_in_view_radius = Physics.OverlapSphere(enemy.transform.position, enemy.enemy_field_view.view_radius, LayerMask.GetMask("Enemy", "Corpses"));

            for (int i = 0; i < targets_in_view_radius.Length; i++)
            {
                Transform target = targets_in_view_radius[i].transform;
                Vector3 direction = (target.position - enemy.transform.position).normalized;
                if (Vector3.Angle(enemy.transform.forward, direction) < enemy.enemy_field_view.view_angle / 2)
                {
                    float distance_to_target = Vector3.Distance(enemy.transform.position, target.position);

                    if (!Physics.Raycast(enemy.transform.position, direction, distance_to_target, LayerMask.NameToLayer("Environment")))
                    {
                        if(targets_in_view_radius[i].tag.Equals(Tags.corpse))
                        {
                           
                            Vector3 dir = (enemy.transform.position - enemy.last_spotted_position.LastPosition).normalized;
                            enemy.agent.destination = enemy.agent.destination + (dir * 4.0f);
                            enemy.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
                            corpse_identification = true;
                            enemy.enemy_manager.list_of_corpses.Add(targets_in_view_radius[i].gameObject);
                            break;
                        }                            
                    }
                }
            }           
        }
    }
}
