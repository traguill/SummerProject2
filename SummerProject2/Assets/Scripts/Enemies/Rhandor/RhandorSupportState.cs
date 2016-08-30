using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RhandorSupportState : IRhandorStates
{
    private readonly RhandorController rhandor;
    private float max_time_for_support, tfs_timer;
    private bool on_zone;

    public RhandorSupportState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public void StartState()
    {
        max_time_for_support = 10.0f;
        tfs_timer = 0.0f;
        on_zone = false;

        rhandor.agent.destination = rhandor.last_spotted_position.LastPosition;
        rhandor.agent.Resume();
    }

    public void UpdateState()
    {
        if(on_zone)
        {
            if (tfs_timer > max_time_for_support)
            {
                if (rhandor.alarm_system.isAlarmActive())
                    ToAlertState();
                else
                {
                    if (rhandor.static_neutral)
                        ToIdleState();
                    else
                        ToPatrolState();
                }
            }
            else
                tfs_timer += Time.deltaTime;                
        }
        else
        {
            ApproachingToSpottedZone();
        }
            
    }

    public void ToIdleState()
    {
        rhandor.ChangeStateTo(rhandor.idle_state);
    }

    public void ToPatrolState()
    {
        rhandor.ChangeStateTo(rhandor.patrol_state);
    }

    public void ToAlertState()
    {
        rhandor.ChangeStateTo(rhandor.alert_state);
    }

    public void ToSpottedState()
    {
        rhandor.ChangeStateTo(rhandor.spotted_state);
    }

    public void ToCorpseState()
    {
        rhandor.ChangeStateTo(rhandor.corpse_state);
    }

    public void ToSupportState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state SUPPORT");
    }

    private void ApproachingToSpottedZone()
    {
        float distance_to_target = Vector3.Distance(rhandor.transform.position, rhandor.agent.destination);
        float min_dist = 6.0f;

        if (distance_to_target < min_dist)
        {
            on_zone = true;
            Vector3 direction = (rhandor.agent.destination - rhandor.transform.position).normalized;
            rhandor.agent.destination += (-direction * min_dist * Random.value);
        }
    }
}

