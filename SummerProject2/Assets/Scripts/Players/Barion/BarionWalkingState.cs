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

    public void StartState()
    { }

    public void UpdateState()
    {
        //Check arrive destination
        if (barion.DstArrived())
            ToIdleState();

        //Actions performed when Barion is selected
        if(barion.is_selected)
        {
            //Check new input movement
            if (barion.GetMovement(ref destination))
                ToWalkingState();

            //Can transition to moving box
            if (barion.target_box != null)
            {
                ToMoveBoxState();
            }

            if(Input.GetAxis("Ability1") != 0 && barion.cooldown_inst.AbilityIsReady(1))
            {
                ToInvisibleSphereState();
            }
        }
        
    }

    public void ToIdleState()
    {
        barion.ChangeStateTo(barion.idle_state);
    }

    public void ToMoveBoxState()
    {
        barion.ChangeStateTo(barion.moving_box_state);
    }

    public void ToWalkingState()
    {
        //Resets destination to a new one
        barion.agent.SetDestination(destination);
    }

    public void ToHideState()
    {
        barion.ChangeStateTo(barion.hiding_state);
    }

    public void ToInvisibleSphereState()
    {
        barion.StopMovement();
        barion.ChangeStateTo(barion.invisible_sphere_state);
    }


}
