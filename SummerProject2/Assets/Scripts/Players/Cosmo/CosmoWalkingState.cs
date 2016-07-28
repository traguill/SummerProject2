using UnityEngine;
using System.Collections;
using System;

public class CosmoWalkingState : ICosmoState
{
    private readonly CosmoController cosmo;

    private Vector3 destination = new Vector3(); //Pathfinding destination

    public CosmoWalkingState(CosmoController cosmo_controller)
    {
        cosmo = cosmo_controller;
    }

    public void StartState()
    {}

    public void UpdateState()
    {
        Transitions();
       
    }

    public void ToIdleState()
    {
        cosmo.ChangeStateTo(cosmo.idle_state);
        cosmo.StopMovement();
    }

    public void ToSensorialState()
    {
        cosmo.ChangeStateTo(cosmo.sensorial_state);
        cosmo.StopMovement();
    }

    public void ToWalkingState()
    {
        cosmo.agent.SetDestination(destination); //Change destination to new one.
    }

    /// <summary>
    /// Checks all conditions to transition to another state
    /// </summary>
    void Transitions()
    {
        //To IDLE (destination arrived?)
        if (cosmo.DstArrived())
            ToIdleState();

        //Actions performed when it's selected
        if (cosmo.is_selected)
        {
            //To WALKING (another destination)
            if (cosmo.GetMovement(ref destination))
            {
                ToWalkingState();
                return;
            }

            //To Ability1: sensorial
            if (Input.GetAxis("Ability1") != 0 && cosmo.cooldown_inst.AbilityIsReady(1))
            {
                ToSensorialState();
                return;
            }
        }
    }

    public void ToHideState()
    {
        
    }

    
}
