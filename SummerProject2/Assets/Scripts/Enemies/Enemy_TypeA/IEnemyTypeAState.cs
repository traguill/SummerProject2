using UnityEngine;
using System.Collections;

public interface IEnemyTypeAstates
    {
    void UpdateState();

    void ToIdleState();

    void ToWalkingState();

    void ToMoveBoxState();

}

