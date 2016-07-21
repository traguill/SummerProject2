using UnityEngine;
using System.Collections;
using System;

public class BarionMovingBoxState : IBarionState {

    private readonly BarionController barion;

    public BarionMovingBoxState(BarionController barion_controller)
    {
        barion = barion_controller;
    }

    public void UpdateState()
    {
        throw new NotImplementedException();
    }

    public void ToIdleState()
    {
        throw new NotImplementedException();
    }

    public void ToMoveBoxState()
    {
        throw new NotImplementedException();
    }

    public void ToWalkingState()
    {
        throw new NotImplementedException();
    }

    

}
