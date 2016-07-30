using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public float max_angle;
    public float rotation_speed;
    [HideInInspector] public Transform camera_lens;

    //State machine
    [HideInInspector] public ICameraStates current_state;
    [HideInInspector] public CameraIdleState idle_state;
    [HideInInspector] public CameraAlertState alert_state;
    [HideInInspector] public CameraFollowingState following_state;

    void Awake()
    {
        // State machine
        // -- IDLE --
        idle_state = new CameraIdleState(this);
        // -- ALERT --
        alert_state = new CameraAlertState(this);
        // -- FOLLOWING --
        following_state = new CameraFollowingState(this);

        camera_lens = GameObject.Find("camera_lens").transform;
    }

    void Start()
    {
        ChangeStateTo(idle_state);
    }

    void Update()
    {
        current_state.UpdateState();
    }

    public void ChangeStateTo(ICameraStates new_state)
    {
        current_state = new_state;
        current_state.StartState();
    }
}


