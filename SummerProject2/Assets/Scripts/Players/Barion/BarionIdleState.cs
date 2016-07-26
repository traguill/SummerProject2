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

    public void StartState()
    { }

    public void UpdateState()
    {
        //If it's not selected no action is performed
        if (barion.is_selected == false)
            return;
        //Can transition to walking
        if (barion.GetMovement(ref destination))
        {
            ToWalkingState();
        }

        //Can transition to moving box
        if (barion.target_box != null)
        {
            ToMoveBoxState();
        }
    }

    public void ToIdleState()
    {
        Debug.Log("Barion: can't transition to same state IDLE");
    }

    public void ToMoveBoxState()
    {
        barion.ChangeStateTo(barion.moving_box_state);
    }

    public void ToWalkingState()
    {
        barion.ChangeStateTo(barion.walking_state);
        barion.agent.SetDestination(destination);
    }

    public void ToHideState()
    {
        barion.ChangeStateTo(barion.hiding_state);
    }

    

}
