using UnityEngine;
using System.Collections;

public class CosmoChainedState : ICosmoState {

    private readonly CosmoController cosmo;

    Transform barion_position;

    Vector3 dst_to_barion;

    NavMeshAgent agent;

    Vector3 destination = new Vector3(); //Pathfinding

    public CosmoChainedState(CosmoController cosmo_controller, Transform barion)
    {
        cosmo = cosmo_controller;
        barion_position = barion;
        agent = cosmo.agent;
    }

    public void StartState()
    {
        //Calculate vector from barion
        dst_to_barion = cosmo.transform.position - barion_position.position;
    }

    public void UpdateState()
    {
        //Mantain the vector distance from barion
        Vector3 position = barion_position.position + dst_to_barion;
        cosmo.transform.position = position;
        agent.Warp(position);

        Transitions();
    }

    private void Transitions()
    {
        if (cosmo.is_selected == false)
            return;

        //To WALKING
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

        //To Ability2: portals
        if (Input.GetAxis("Ability2") != 0)
        {
            ToPortalState();
            return;
        }
    }

    public void ToIdleState()
    {
        cosmo.ChangeStateTo(cosmo.idle_state);
    }

    public void ToWalkingState()
    {
        agent.SetDestination(destination);
        cosmo.ChangeStateTo(cosmo.walking_state);
    }

    public void ToSensorialState()
    {
        cosmo.ChangeStateTo(cosmo.sensorial_state);
    }

    public void ToHideState()
    {
        Debug.Log("TODO: Cosmo to hide state");
    }

    public void ToPortalState()
    {
        cosmo.ChangeStateTo(cosmo.portal_state);
    }

    public void ToChainedState()
    {
        Debug.Log("Cosmo can't transition from CHAINED to CHAINED");
    }
}
