using UnityEngine;
using System.Collections;
using System;

public class CosmoIdleState : ICosmoState
{
    private readonly CosmoController cosmo;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    public CosmoIdleState(CosmoController cosmo_controller)
    {
        cosmo = cosmo_controller;
    }

    public void StartState()
    {}

    public void UpdateState()
    {
        //If it's not selected no action is performed.
        if (cosmo.is_selected == false)
            return;

        Transitions();
    }

    public void ToIdleState()
    {
        Debug.Log("Cosmo can't transition from IDLE state to IDLE");
    }

    public void ToSensorialState()
    {
        cosmo.current_state = cosmo.sensorial_state;
        cosmo.current_state.StartState();
    }

    public void ToWalkingState()
    {
        cosmo.current_state = cosmo.walking_state;
        cosmo.agent.SetDestination(destination);
    }

    
    /// <summary>
    /// Checks the conditons to transition to another state.
    /// </summary>
    void Transitions()
    {
        //To WALKING
        if(cosmo.GetMovement(ref destination))
        {
            ToWalkingState();
            return;
        }

        //To Ability1: sensorial
        if(Input.GetAxis("Ability1") != 0)
        {
            ToSensorialState();
            return;
        }
        
    }

    
}
