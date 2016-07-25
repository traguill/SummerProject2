using UnityEngine;
using System.Collections;

public class EnemyAController : MonoBehaviour {

    public GameObject neutral_path, alert_path;
    [HideInInspector] public Transform[] neutral_patrol, alert_patrol;
    [HideInInspector] public int current_position;

    //State machine
    [HideInInspector] public IEnemyAStates current_state;
    [HideInInspector] public EnemyAIdleState idle_state;
    [HideInInspector] public EnemyAPatrolState patrol_state;
    [HideInInspector] public EnemyAAlertState alert_state;

    void Awake()
    {
        // State machine

        // -- IDLE --
        idle_state = new EnemyAIdleState(this);
        // -- PATROL --
        patrol_state = new EnemyAPatrolState(this);
        neutral_patrol = patrol_state.AwakeState();
        patrol_state.StartState();
        // -- ALERT --
        alert_state = new EnemyAAlertState(this);
        alert_patrol = alert_state.AwakeState();
    }
    
    void Start()
    {
        current_state = patrol_state;
    }

    void Update()
    {
        current_state.UpdateState();
    }
}

