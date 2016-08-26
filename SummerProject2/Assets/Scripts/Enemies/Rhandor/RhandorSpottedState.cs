﻿using UnityEngine;
using System.Collections;

public class RhandorSpottedState : IRhandorStates
{
    
    private readonly RhandorController rhandor;   
    private float time_searching, max_time_searching;
    private bool element_identification = false;        // When Rhandor is close enought, it will trigger the alarm upon element
                                                        // (portal, corpse,...) identification.

    public RhandorSpottedState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
        max_time_searching = 2.5f;  
    }

    public void StartState()
    {
        time_searching = 0.0f;
        element_identification = false;
        rhandor.agent.destination = rhandor.last_spotted_position.LastPosition;
        rhandor.agent.speed = rhandor.spotted_speed;
    }

    public void UpdateState()
    {
        // Rhandor checks distance to recalculate the final destination, to not stop over the corpse.
        // The alarm could be activated in the same moment that is approaching.
        if (!element_identification)
        {
            ElementSpotted();
        }

        if(rhandor.agent.remainingDistance < rhandor.agent.stoppingDistance)
        {
            if (time_searching > max_time_searching)
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
                time_searching += Time.deltaTime;
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

    private void ElementSpotted()
    {
        float min_dist = 8.0f;
        if (Vector3.Distance(rhandor.agent.destination, rhandor.transform.position) < min_dist)
        {
            Vector3 direction = (rhandor.spotted_element.transform.position - rhandor.transform.position).normalized;
            if (Vector3.Angle(rhandor.transform.forward, direction) < rhandor.enemy_field_view.view_angle / 2)
            {
                float distance_to_target = Vector3.Distance(rhandor.transform.position, rhandor.spotted_element.transform.position);
                if (!Physics.Raycast(rhandor.transform.position, direction, distance_to_target, LayerMask.GetMask("Environment", "InvisibleShield")))
                {
                    if (rhandor.spotted_element.tag.Equals(Tags.corpse))
                    {
                        Vector3 dir = (rhandor.transform.position - rhandor.last_spotted_position.LastPosition).normalized;
                        rhandor.agent.destination = rhandor.agent.destination + (dir * 4.0f);
                        rhandor.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
                        element_identification = true;
                        rhandor.enemy_manager.list_of_corpses.Add(rhandor.spotted_element);
                    }

                    if (rhandor.spotted_element.tag.Equals(Tags.portal))
                    {
                        Vector3 dir = (rhandor.transform.position - rhandor.last_spotted_position.LastPosition).normalized;
                        rhandor.agent.destination = rhandor.agent.destination + (dir * 4.0f);
                        rhandor.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
                        element_identification = true;
                        //rhandor.enemy_manager.list_of_corpses.Add(rhandor.spotted_element);
                    }
                }


                //        Collider[] targets_in_view_radius = Physics.OverlapSphere(rhandor.transform.position, rhandor.enemy_field_view.view_radius, LayerMask.GetMask("Enemy", "Corpses", "SelectableObject"));

                //    for (int i = 0; i < targets_in_view_radius.Length; i++)
                //    {
                //        Transform target = targets_in_view_radius[i].transform;
                //        Vector3 direction = (target.position - rhandor.transform.position).normalized;
                //        if (Vector3.Angle(rhandor.transform.forward, direction) < rhandor.enemy_field_view.view_angle / 2)
                //        {
                //            float distance_to_target = Vector3.Distance(rhandor.transform.position, target.position);

                //            if (!Physics.Raycast(rhandor.transform.position, direction, distance_to_target, LayerMask.NameToLayer("Environment")))
                //            {
                //                if(targets_in_view_radius[i].tag.Equals(Tags.corpse))
                //                {
                //                    Vector3 dir = (rhandor.transform.position - rhandor.last_spotted_position.LastPosition).normalized;
                //                    rhandor.agent.destination = rhandor.agent.destination + (dir * 4.0f);
                //                    rhandor.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
                //                    element_identification = true;
                //                    rhandor.enemy_manager.list_of_corpses.Add(targets_in_view_radius[i].gameObject);
                //                    break;
                //                }                            
                //            }
                //        }
                //    }           
                //}
            }
        }
    }
}
