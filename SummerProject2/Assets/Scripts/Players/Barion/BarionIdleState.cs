using UnityEngine;
using System.Collections;
using System;

public class BarionIdleState : IBarionState {

    private readonly BarionController barion;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    public BarionIdleState(BarionController barion_controller)
    {
        barion = barion_controller;
    }

    public void UpdateState()
    {
        //Check if its selected to perform actions
        if (barion.GetMovement(ref destination))
            ToWalkingState();
    }

    public void ToIdleState()
    {
        Debug.Log("Barion: can't transition to same state IDLE");
    }

    public void ToMoveBoxState()
    {
        barion.current_state = barion.moving_box_state;
    }

    public void ToWalkingState()
    {
        barion.current_state = barion.walking_state;
        barion.agent.SetDestination(destination);
    }

    

}
