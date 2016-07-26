using UnityEngine;
using System.Collections;

public interface ICosmoState
{
    void StartState(); //Must be called when changing state to reset all values.

    void UpdateState();

    void ToIdleState();

    void ToWalkingState();

    void ToSensorialState();

    void ToHideState();
}