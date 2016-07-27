using UnityEngine;
using System.Collections;

public interface INyxState
{
    void StartState(); //Must be called when changing state to reset all values.

    void UpdateState();

    void ToIdleState();

    void ToWalkingState();

    void ToKillingState();

    void ToHideState();

    void ToDashState();
}
