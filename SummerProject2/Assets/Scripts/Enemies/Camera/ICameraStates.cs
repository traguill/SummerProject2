using UnityEngine;
using System.Collections;

public interface ICameraStates
{
    void StartState();

    void UpdateState();

    void ToIdleState();

    void ToAlertState();

    void ToFollowingState();
}
