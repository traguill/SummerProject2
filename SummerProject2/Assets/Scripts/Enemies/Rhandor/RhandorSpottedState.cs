using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RhandorSpottedState : IRhandorStates
{
    private readonly RhandorController rhandor;
    private float time_between_identifications, tbi_timer; 
    private float last_time_identification, lti_timer;
    private bool element_identified;

    public RhandorSpottedState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public void StartState()
    {
        lti_timer = tbi_timer = 0.0f;
        last_time_identification = 4.0f;
        time_between_identifications = 2.50f;

        element_identified = false;

        rhandor.agent.destination = rhandor.enemy_field_view.queue_of_spotted_elements.Peek().transform.position;
        rhandor.agent.speed = rhandor.spotted_speed;
    }

    public void UpdateState()
    {
        Queue<GameObject> queue = rhandor.enemy_field_view.queue_of_spotted_elements;

        if(element_identified)
        {
            if (tbi_timer > time_between_identifications)
            {
                rhandor.enemy_field_view.queue_of_spotted_elements.Dequeue();
                if(queue.Count > 0)
                {
                    rhandor.agent.destination = queue.Peek().transform.position;
                    tbi_timer = 0.0f;
                    element_identified = false;
                }
                else if(last_time_identification > lti_timer)
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
                    lti_timer += Time.deltaTime;
            }
            else
            {
                tbi_timer += Time.deltaTime;
            }
        }  
        else
        {
            if (queue.Count > 0)
                ElementSpotted(rhandor.enemy_field_view.queue_of_spotted_elements.Peek());
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
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state SPOTTED");
    }

    public void ToCorpseState()
    {
        rhandor.ChangeStateTo(rhandor.corpse_state);
    }

    private void ElementSpotted(GameObject element_spotted)
    {
        float distance_to_target = Vector3.Distance(rhandor.transform.position, element_spotted.transform.position);
        float min_dist = 6.0f;

        if (distance_to_target < min_dist)
        {
            Vector3 direction = (element_spotted.transform.position - rhandor.transform.position).normalized;
            if (Vector3.Angle(rhandor.transform.forward, direction) < rhandor.enemy_field_view.view_angle / 2)
            {                
                if(Physics.Raycast(rhandor.transform.position, direction, distance_to_target))
                {
                    rhandor.agent.destination = element_spotted.transform.position + (-direction * min_dist * 0.4f);                    
                    rhandor.enemy_field_view.list_of_spotted_elements.Add(element_spotted);
                    element_identified = true;
                    rhandor.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
                }
            }
        }
    }
}
