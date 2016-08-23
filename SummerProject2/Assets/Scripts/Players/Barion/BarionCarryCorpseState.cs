using UnityEngine;
using System.Collections;

public class BarionCarryCorpseState : IBarionState 
{
    private readonly BarionController barion;

    public  BarionCarryCorpseState(BarionController barion_controller)
    {
        barion = barion_controller;
    }

    public void StartState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public void ToIdleState()
    {
        throw new System.NotImplementedException();
    }

    public void ToWalkingState()
    {
        throw new System.NotImplementedException();
    }

    public void ToMoveBoxState()
    {
        throw new System.NotImplementedException();
    }

    public void ToHideState()
    {
        throw new System.NotImplementedException();
    }

    public void ToInvisibleSphereState()
    {
        throw new System.NotImplementedException();
    }

    public void ToShieldState()
    {
        throw new System.NotImplementedException();
    }

    public void ToCarryCorpseState()
    {
        throw new System.NotImplementedException();
    }
}
