using UnityEngine;
using System.Collections;
using System;

public class NyxIdleState : INyxState
{
    private readonly NyxController nyx;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    public NyxIdleState(NyxController nyx_controller)
    {
        nyx = nyx_controller;
    }

    public void StartState()
    {
    }

    public void UpdateState()
    {
        //If it's not selected no action is performed.
        if (nyx.is_selected == false)
            return;

        Transitions();
    }

    public void ToIdleState()
    {
        Debug.Log("Nyx can't transition from IDLE state to IDLE");
    }

    public void ToKillingState()
    {
        nyx.ChangeStateTo(nyx.killing_state);
    }

    public void ToWalkingState()
    {
        nyx.agent.SetDestination(destination);
        nyx.ChangeStateTo(nyx.walking_state);
    }

    public void ToHideState()
    {
        nyx.ChangeStateTo(nyx.hiding_state);
    }

    /// <summary>
    /// Checks the conditons to transition to another state.
    /// </summary>
    void Transitions()
    {
        //To KILLING
        if (nyx.KillEnemy())
        {
            ToKillingState();
            return;
        }

        //To WALKING
        if (nyx.GetMovement(ref destination))
        {
            ToWalkingState();
            return;
        }

        //To DASH
        if(Input.GetKeyUp(KeyCode.Q) && nyx.cooldown_inst.AbilityIsReady(1)) //TODO: change getkeyup for ability 1 axis
        {
            ToDashState();
            return;
        }

        //To DEATH_TRAP
        if(Input.GetKeyUp(KeyCode.W) && nyx.cooldown_inst.AbilityIsReady(2)) //TODO: change getkeyup for ability2 and add cooldown
        {
            ToDeathTrapState();
            return;
        }

    }

    public void ToDashState()
    {
        nyx.ChangeStateTo(nyx.dash_state);
    }

    public void ToDeathTrapState()
    {
        nyx.ChangeStateTo(nyx.death_trap_state);
    }

   

}
