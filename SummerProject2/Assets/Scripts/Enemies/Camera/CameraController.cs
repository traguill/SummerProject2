using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ANGLE_DIR
{
    FORWARD,
    RIGHT,
    LEFT
};

public class CameraController : MonoBehaviour
{
    public bool static_camera;
    public float angle_range;
    public float max_rotation_speed, min_rotation_speed;
    public float seconds_from_last_sight;   

    //Camera sweep movement
    private bool returning = false;  // Useful to separate each camera direction
    [HideInInspector] public float current_angle, current_speed, min_speed;
    [HideInInspector] public Vector3 initial_forward_direction;
    [HideInInspector] public float mid_angle;

    //State machine
    [HideInInspector] public ICameraStates current_state;
    [HideInInspector] public CameraIdleState idle_state;
    [HideInInspector] public CameraAlertState alert_state;
    [HideInInspector] public CameraFollowingState following_state;

    // Script references
    [HideInInspector] public AlarmSystem alarm_system;
    [HideInInspector] public LastSpottedPosition last_spotted_position;
    [HideInInspector] public GameObject[] players;
    [HideInInspector] public Transform camera_lens;

    private Vector3 far_position;
    private Vector3[] vertices;

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

        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        last_spotted_position = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<LastSpottedPosition>();
        players = GameObject.FindGameObjectsWithTag(Tags.player);      
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

    public void CameraSweep()
    {
        if (returning)
        {
            if (current_angle < mid_angle)
                current_speed = Mathf.Lerp(current_speed, min_rotation_speed, 0.02f);
            else
                current_speed = Mathf.Lerp(current_speed, max_rotation_speed, 0.02f);

            current_angle -= current_speed * Time.deltaTime;
            camera_lens.transform.eulerAngles += new Vector3(0.0f, -current_speed * Time.deltaTime, 0.0f);
        }
        else
        {
            if (current_angle > mid_angle)
                current_speed = Mathf.Lerp(current_speed, min_rotation_speed, 0.02f);
            else
                current_speed = Mathf.Lerp(current_speed, max_rotation_speed, 0.02f);

            current_angle += current_speed * Time.deltaTime;
            camera_lens.transform.eulerAngles += new Vector3(0.0f, current_speed * Time.deltaTime, 0.0f);
        }

        if (returning && current_angle < 1.0f || !returning && Mathf.Abs(current_angle - angle_range) < 1.0f)
            returning = !returning;
    }

    public ANGLE_DIR AngleDir(Vector3 fwd, Vector3 target_direction, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, target_direction);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0)
        {
            return ANGLE_DIR.RIGHT;
        }
        else if (dir < 0.0)
        {
            return ANGLE_DIR.LEFT;
        }
        else
        {
            return ANGLE_DIR.FORWARD;
        }
    }
}


