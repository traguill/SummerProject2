using UnityEngine;
using System.Collections;
using System;

public class BarionWalkingState : IBarionState {

    private readonly BarionController barion;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    public BarionWalkingState(BarionController barion_controller)
    {
        barion = barion_controller;
    }

    public void UpdateState()
    {
        //Check arrive destination
        if (barion.DstArrived())
            ToIdleState();

        //Check new input movement
        if (barion.GetMovement(ref destination))
            ToWalkingState();

        //Check new input box selection
    }

    public void ToIdleState()
    {
        barion.current_state = barion.idle_state;
    }

    public void ToMoveBoxState()
    {
        barion.current_state = barion.moving_box_state;
    }

    public void ToWalkingState()
    {
        //Resets destination to a new one
        barion.agent.SetDestination(destination);
    }

    

}
