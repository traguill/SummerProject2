﻿using UnityEngine;
using System.Collections;

public class RhandorController : Enemies {

    public GameObject neutral_path, alert_path;
    [HideInInspector] public Transform[] neutral_patrol, alert_patrol;
    [HideInInspector] public int current_position;

    // For corpses representation
    [HideInInspector] public SpriteRenderer render;

    // NavMeshAgent variables
    float patrol_speed, alert_speed;

    //State machine
    [HideInInspector] public IRhandorStates current_state;
    [HideInInspector] public RhandorIdleState idle_state;
    [HideInInspector] public RhandorPatrolState patrol_state;
    [HideInInspector] public RhandorAlertState alert_state;
    [HideInInspector] public RhandorSpottedState spotted_state;
    [HideInInspector] public RhandorCorpseState corpse_state;

    void Awake()
    {
        // State machine
        // -- IDLE --
        idle_state = new RhandorIdleState(this);
        // -- PATROL --
        patrol_state = new RhandorPatrolState(this);
        neutral_patrol = patrol_state.AwakeState();  // It transforms the path GameObject to Transform[]
        // -- ALERT --
        alert_state = new RhandorAlertState(this);
        alert_patrol = alert_state.AwakeState();     // It transforms the path GameObject to Transform[] 
        // -- SPOTTED --
        spotted_state = new RhandorSpottedState(this);
        // -- CORPSE --
        corpse_state = new RhandorCorpseState(this);

        render = GetComponent<SpriteRenderer>();
        type = ENEMY_TYPES.RHANDOR;
    }
    
    void Start()
    {
        ChangeStateTo(patrol_state);
    }

    void Update()
    {
        current_state.UpdateState();
    }

    public void ChangeStateTo(IRhandorStates new_state)
    {
        current_state = new_state;
        current_state.StartState();
    }

    public void Dead()
    {
        if (current_state != corpse_state)
            ChangeStateTo(corpse_state);
        else
            Debug.Log("The enemy is already dead!");
    }
}

