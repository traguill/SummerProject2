using UnityEngine;
using System.Collections;

public class NyxChainedState : INyxState {

    private readonly NyxController nyx;

    Transform barion_position;

    Vector3 dst_to_barion;

    NavMeshAgent agent;

    Vector3 destination = new Vector3(); //Pathfinding

    public NyxChainedState(NyxController nyx_controller, Transform barion)
    {
        nyx = nyx_controller;
        barion_position = barion;
        agent = nyx.agent;
    }

    public void StartState()
    {
        //Calculate vector from barion
        dst_to_barion = nyx.transform.position - barion_position.position;
    }

    public void UpdateState()
    {
        //Mantain the vector distance from barion
        Vector3 position = barion_position.position + dst_to_barion;
        nyx.transform.position = position;
        agent.Warp(position);

        Transitions();
    }

    private void Transitions()
    {
        if (nyx.is_selected == false)
            return;

        //To KILLING
        if (nyx.KillEnemy())
        {
            ToKillingState();
            return;
        }

        //To WALKING
        if (nyx.GetMovement(ref destination))
        {
            ToWalkingState();
            return;
        }

        //To DASH
        if (Input.GetKeyUp(KeyCode.Q) && nyx.cooldown_inst.AbilityIsReady(1)) //TODO: change getkeyup for ability 1 axis
        {
            ToDashState();
            return;
        }

        //To DEATH_TRAP
        if (Input.GetKeyUp(KeyCode.W) && nyx.cooldown_inst.AbilityIsReady(2)) //TODO: change getkeyup for ability2 and add cooldown
        {
            ToDeathTrapState();
            return;
        }
    }

    public void ToIdleState()
    {
        nyx.ChangeStateTo(nyx.idle_state);
    }

    public void ToWalkingState()
    {
        nyx.agent.SetDestination(destination);
        nyx.ChangeStateTo(nyx.walking_state);
    }

    public void ToKillingState()
    {
        nyx.ChangeStateTo(nyx.killing_state);
    }

    public void ToHideState()
    {
        nyx.ChangeStateTo(nyx.hiding_state);
    }

    public void ToDashState()
    {
        nyx.ChangeStateTo(nyx.dash_state);
    }

    public void ToDeathTrapState()
    {
        nyx.ChangeStateTo(nyx.death_trap_state);
    }

    public void ToChainedState()
    {
        Debug.Log("Nyx can't transition from CHAINED to CHAINED");
    }
}
