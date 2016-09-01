using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*  --- Spotted State ---
 * 
 *  When some Rhandor on the level detects an unknown element (corpse, portal, etc...), it will change to that
 *  state. The Rhandor assigned to spotted state will move the clearly identify that element. If the identification
 *  is correct, the alarm will be triggered and will ask for help to other Rhandors that are near him (variables
 *  related: ask_for_help_radius and max_num_of_helpers on RhandorController. 
 * 
 */

public class RhandorSpottedState : IRhandorStates
{
    private readonly RhandorController rhandor;
    private float time_between_identifications, tbi_timer;      // Waiting time between identification
    private float last_time_identification, lti_timer;          // Time to return to default behaviour after last identification

    // Variables use on the continuos element identification
    private bool element_identified;
    Queue<GameObject> queue;

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
        queue = rhandor.enemy_field_view.pending_identification_elements;

        rhandor.agent.speed = rhandor.spotted_speed;
        rhandor.agent.SetDestination(queue.Peek().transform.position);
        rhandor.agent.Resume();       
    }

    public void UpdateState()
    {
        
        if(element_identified)
        {
            if (tbi_timer > time_between_identifications)
            {
                queue.Dequeue();
                if(queue.Count > 0)
                {
                    rhandor.agent.destination = queue.Peek().transform.position;
                    tbi_timer = 0.0f;
                    element_identified = false;
                }
                else if(last_time_identification > lti_timer)
                    rhandor.ReturnDefaultBehaviour();
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
                ElementSpotted(queue.Peek());
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

    public void ToSupportState()
    {
        rhandor.ChangeStateTo(rhandor.support_state);
    }   

    public void ToCorpseState()
    {
        rhandor.ChangeStateTo(rhandor.corpse_state);
    }

    public void ToSpottedState()
    {
        Debug.Log("Enemy" + rhandor.name + "can't transition to same state SPOTTED");
    }

    /// <summary>
    /// ElementSpotted do the "Identification" of the unknown element. When the Rhandor is close enough, it will raycast that
    /// element. If there is impact and it is successful (checking its layer), Rhandor will continue its analysis to the next 
    /// spotted_element.
    /// </summary>
    /// <param name="element_spotted"></param>
    private void ElementSpotted(GameObject element_spotted)
    {
        float distance_to_target = Vector3.Distance(rhandor.transform.position, element_spotted.transform.position);
        float min_dist = 6.0f;

        if (distance_to_target < min_dist)
        {
            Vector3 direction = (element_spotted.transform.position - rhandor.transform.position).normalized;
            if (Vector3.Angle(rhandor.transform.forward, direction) < rhandor.enemy_field_view.view_angle / 2)
            {
                if (Physics.Raycast(rhandor.transform.position, direction, distance_to_target, 1 << element_spotted.layer))
                {
                     rhandor.agent.SetDestination(rhandor.agent.destination + (-direction * min_dist * Random.value));                    
                    rhandor.enemy_manager.list_of_spotted_elements.Add(element_spotted);
                    element_identified = true;
                    rhandor.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);

                    AskForHelp();
                }
            }
        }
    }

    private void AskForHelp()
    {
        int num_of_helpers = 0;
        foreach (GameObject enemy in rhandor.enemy_manager.enemies)
        {
            if (enemy == rhandor.gameObject)
                continue;

            if (num_of_helpers < rhandor.max_num_of_helpers && 
                Vector3.Distance(enemy.transform.position, rhandor.last_spotted_position.LastPosition) < rhandor.ask_for_help_radius)
            {
                RhandorController enemy_controller = enemy.GetComponent<RhandorController>();
                ++num_of_helpers;
                enemy_controller.ChangeStateTo(enemy_controller.support_state);
            }
        }
    }
}
