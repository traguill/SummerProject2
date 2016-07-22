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
    {}

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
        nyx.current_state = nyx.killing_state;
        nyx.current_state.StartState();
    }

    public void ToWalkingState()
    {
        nyx.current_state = nyx.walking_state;
        nyx.agent.SetDestination(destination);
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

        
    }

}
