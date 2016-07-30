using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public bool static_camera;
    public float angle_range;
    public float max_rotation_speed, min_rotation_speed;
    public float seconds_from_last_sight;
    [HideInInspector] public Transform camera_lens;
    [HideInInspector] public Vector3 initial_forward_direction;
    [HideInInspector] public float mid_angle;

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
        initial_forward_direction = camera_lens.transform.forward;
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


