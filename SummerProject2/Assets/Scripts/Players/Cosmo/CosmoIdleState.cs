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
        cosmo.ChangeStateTo(cosmo.sensorial_state);
    }

    public void ToWalkingState()
    {
        cosmo.ChangeStateTo(cosmo.walking_state);
        cosmo.agent.SetDestination(destination);
    }

    public void ToHideState()
    {

    }

   public void ToPortalState()
    {
        cosmo.ChangeStateTo(cosmo.portal_state);
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
        if(Input.GetAxis("Ability1") != 0 && cosmo.cooldown_inst.AbilityIsReady(1))
        {
            ToSensorialState();
            return;
        }

        //To Ability2: portals
        if (Input.GetAxis("Ability2") != 0)
        {
            ToPortalState();
            return;
        }
        
    }

    
}
