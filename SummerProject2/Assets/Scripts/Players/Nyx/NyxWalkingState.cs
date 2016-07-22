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
        nyx.current_state = nyx.idle_state;
    }

    public void ToKillingState()
    {
        nyx.current_state = nyx.killing_state;
        nyx.current_state.StartState();
    }

    public void ToWalkingState()
    {
        //Resets destination to a new one
        nyx.agent.SetDestination(destination);
    }

    
}
