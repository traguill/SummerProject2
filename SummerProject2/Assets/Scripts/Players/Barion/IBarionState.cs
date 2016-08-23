using UnityEngine;
using System.Collections;

public interface IBarionState
{
    void StartState(); 

    void UpdateState();

    void ToIdleState();

    void ToWalkingState();

    void ToMoveBoxState();

    void ToHideState();

    void ToInvisibleSphereState();

    void ToShieldState();

    void ToCarryCorpseState();

}
