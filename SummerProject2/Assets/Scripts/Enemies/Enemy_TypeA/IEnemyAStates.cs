using UnityEngine;
using System.Collections;

public interface IEnemyAStates
    {

    void StartState();

    void UpdateState();

    void ToIdleState();

    void ToPatrolState();

    void ToAlertState();

}

