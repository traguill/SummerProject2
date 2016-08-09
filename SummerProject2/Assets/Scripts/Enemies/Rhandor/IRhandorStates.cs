using UnityEngine;
using System.Collections;

public interface IRhandorStates
{

    void StartState();

    void UpdateState();

    void ToIdleState();

    //void ToSpotState();

    void ToPatrolState();

    void ToAlertState();

    void ToCorpseState();

}

