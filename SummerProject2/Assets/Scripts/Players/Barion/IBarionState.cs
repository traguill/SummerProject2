using UnityEngine;
using System.Collections;

public interface IBarionState
{
    void UpdateState();

    void ToIdleState();

    void ToWalkingState();

    void ToMoveBoxState();

}
