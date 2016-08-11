using UnityEngine;
using System.Collections;
using System;

public class NyxKillingState : INyxState
{
    private readonly NyxController nyx;

    GameObject enemy; //Enemy to kill.
    Transform kill_point; //Point of the enemy to reach before killing.

    bool enemy_near; //Is the enemy near enough to kill it?

    float kill_stopping_distance = 3;

    public NyxKillingState(NyxController nyx_controller)
    {
        nyx = nyx_controller;
    }
    public void StartState()
    {
        enemy = nyx.target_to_kill;
        kill_point = enemy.transform.FindChild("KillPoint");
        enemy_near = false;
    }

    public void UpdateState()
    {
        if(enemy_near == false) //Moving to the enemy
        {
            
            if (Vector3.Distance(nyx.transform.position, kill_point.position) <= kill_stopping_distance)
            {
                enemy_near = true;
            }
            nyx.agent.SetDestination(kill_point.position);
        }
        else //Killing the enemy
        {
            nyx.enemy_manager.DestroyEnemy(enemy);
            ToIdleState();
        }        
    }

    public void ToIdleState()
    {
        nyx.ChangeStateTo(nyx.idle_state);
    }

    public void ToKillingState()
    {
        //For now this doesnt work
    }

    public void ToWalkingState()
    {
        //For now this doesnt work
    }

    public void ToHideState()
    {
        Debug.Log("Nyx can't transition from KILLING to HIDING");
    }

    public void ToDashState()
    {
        Debug.Log("Nyx can't transition from KILLING to DASH");
    }

    public void ToDeathTrapState()
    {
        Debug.Log("Nyx can't transition from KILLING to DEATH_TRAP");
    }

    
}
