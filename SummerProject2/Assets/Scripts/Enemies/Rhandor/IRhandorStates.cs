using UnityEngine;
using System.Collections;

public interface IRhandorStates
{

    void StartState();

    void UpdateState();

    void ToIdleState();

    void ToSpottedState();

    void ToPatrolState();

    void ToAlertState();

    void ToCorpseState();

}

