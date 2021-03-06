﻿using UnityEngine;
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

        if(barion.target_corpse != null)
        {
            ToCarryCorpseState();
        }

        //Invisible sphere ability (ability1)
        if(Input.GetAxis("Ability1") != 0 && barion.cooldown_inst.AbilityIsReady(1))
        {
            ToInvisibleSphereState();
            return;
        }

        //Invisible sphere ability (ability1)
        if (Input.GetAxis("Ability2") != 0 && barion.cooldown_inst.AbilityIsReady(2))
        {
            ToShieldState();
            return;
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
        barion.agent.SetDestination(destination);
        barion.ChangeStateTo(barion.walking_state);
    }

    public void ToHideState()
    {
        barion.ChangeStateTo(barion.hiding_state);
    }

    public void ToInvisibleSphereState()
    {
        barion.ChangeStateTo(barion.invisible_sphere_state);
    }

    public void ToShieldState()
    {
        barion.ChangeStateTo(barion.shield_state);
    }

    public void ToCarryCorpseState()
    {
        barion.ChangeStateTo(barion.carry_corpse_state);
    }

}
