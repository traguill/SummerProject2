using UnityEngine;
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
    {}

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

    
}
