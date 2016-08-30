using UnityEngine;
using System.Collections;

/*  --- Support State ---
 * 
 *  Rhandor enemies will go to the spotted element (portal, corpse, ...whatever) wheter they are
 *  within the radius call of the Rhandor that has spotted these elements. Once Rhandor has arrived to 
 *  the alert zone, he will wait some time (max_time_for_support) and he will return to whatever he was 
 *  doing before the calling for support.* 
 * 
 */

public class RhandorSupportState : IRhandorStates
{
    private readonly RhandorController rhandor;
    private float max_time_for_support, tfs_timer;       // On seconds
    private bool on_zone;                                // Mark whether Rhandor has arrived to the spotted zone

    public RhandorSupportState(RhandorController enemy_controller)
    {
        rhandor = enemy_controller;
    }

    public void StartState()
    {
        max_time_for_support = 5.0f;
        tfs_timer = 0.0f;
        on_zone = false;

        rhandor.agent.speed = rhandor.spotted_speed;
        rhandor.agent.SetDestination(rhandor.last_spotted_position.LastPosition);
        rhandor.agent.Resume();
    }

    public void UpdateState()
    {
        if(on_zone)
        {
            if (tfs_timer > max_time_for_support)
                rhandor.ReturnDefaultBehaviour();
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

    /// <summary>
    ///  ApproachingToSpottedZone sets a new destination position when the Rhandor 
    ///  arrives to the Spotted Zone. That way, the Rhandor will stop AROUND the position 
    ///  of the spotted element and not OVER the spotted element.
    /// </summary>
    private void ApproachingToSpottedZone()
    {
        float distance_to_target = Vector3.Distance(rhandor.transform.position, rhandor.agent.destination);
        float min_dist = 6.0f;

        if (distance_to_target < min_dist)
        {
            on_zone = true;
            Vector3 direction = (rhandor.agent.destination - rhandor.transform.position).normalized;
            rhandor.agent.SetDestination(rhandor.agent.destination + (-direction * min_dist * Random.value));
        }
    }
}

