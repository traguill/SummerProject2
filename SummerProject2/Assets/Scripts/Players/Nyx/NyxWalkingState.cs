﻿using UnityEngine;
using System.Collections;
using System;

public class NyxWalkingState : INyxState
{
    private readonly NyxController nyx;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    public NyxWalkingState(NyxController nyx_controller)
    {
        nyx = nyx_controller;
    }

    
    public void StartState()
    {
        nyx.players_manager.cursor.NyxMoveTo(nyx.agent.destination);
    }

    public void UpdateState()
    {
        //Check arrive destination
        if (nyx.DstArrived())
            ToIdleState();

       if(nyx.is_selected)
        {
            //To KILLING
            if (nyx.KillEnemy())
            {
                ToKillingState();
                return;
            }

            //Check new input movement
            if (nyx.GetMovement(ref destination))
            {
                ToWalkingState();
                return;
            }

            //To DASH
            if (Input.GetKeyUp(KeyCode.Q)  && nyx.cooldown_inst.AbilityIsReady(1)) //TODO: change getkeyup for ability 1 axis
            {
                ToDashState();
                return;
            }

            //To DEATH_TRAP
            if (Input.GetKeyUp(KeyCode.W)  && nyx.cooldown_inst.AbilityIsReady(2)) //TODO: change getkeyup for ability2 and add cooldown
            {
                ToDeathTrapState();
                return;
            }
        }


    }

    public void ToIdleState()
    {
        nyx.ChangeStateTo(nyx.idle_state);
    }

    public void ToKillingState()
    {
        nyx.ChangeStateTo(nyx.killing_state);
    }

    public void ToWalkingState()
    {
        //Resets destination to a new one
        nyx.agent.SetDestination(destination);
        nyx.players_manager.cursor.NyxMoveTo(nyx.agent.destination);
    }

    public void ToHideState()
    {
        nyx.ChangeStateTo(nyx.hiding_state);
    }

    public void ToDashState()
    {
        nyx.StopMovement();
        nyx.ChangeStateTo(nyx.dash_state);
    }

    public void ToDeathTrapState()
    {
        nyx.StopMovement();
        nyx.ChangeStateTo(nyx.death_trap_state);
    }

    public void ToChainedState()
    {
        Debug.Log("Nyx can't transition from WALKING to CHAINED");
    }

    
}
