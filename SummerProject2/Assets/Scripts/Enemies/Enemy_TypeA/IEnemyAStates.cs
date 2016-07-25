using UnityEngine;
using System.Collections;

public interface IEnemyAStates
    {

    void UpdateState();

    void ToIdleState();

    void ToPatrolState();

    void ToAlertState();

}

