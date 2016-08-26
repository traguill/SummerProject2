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
    public bool static_camera;                               // The camera sweeps or not.
    public float angle_range;                                // In degrees. Area that will cover.
    public float max_rotation_speed, min_rotation_speed;
    public float delay_time_sweep;                           // The time the camera waits in order to change its direction;
    public float seconds_from_last_sight;
    public float zone_for_acceleration;

    //Camera sweep movement
    private bool returning = false;                          // Useful to separate each camera direction
    [HideInInspector] public float current_angle, current_speed;
    [HideInInspector] public Vector3 initial_forward_direction;
    [HideInInspector] public Quaternion initial_rotation;
    [HideInInspector] public float mid_angle;
    private bool stopping_phase;
    private float timer = 0.0f;
    private float left_zone, right_zone;    

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

    void Awake()
    {
        // State machine
        // -- IDLE --
        idle_state = new CameraIdleState(this);
        // -- ALERT --
        alert_state = new CameraAlertState(this);
        // -- FOLLOWING --
        following_state = new CameraFollowingState(this);

        foreach(Transform childs in transform.getChilds())
        {
            if (childs.gameObject.name == "camera_lens")
                camera_lens = childs;
        }

        initial_forward_direction = camera_lens.transform.forward;
        initial_rotation = camera_lens.rotation;

        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        last_spotted_position = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<LastSpottedPosition>();
        players = GameObject.FindGameObjectsWithTag(Tags.player);

        // Camera sweep
        mid_angle = angle_range / 2.0f;
        current_speed = max_rotation_speed;
        left_zone = zone_for_acceleration * angle_range;
        right_zone = angle_range - (zone_for_acceleration * angle_range);
    }

    void Start()
    {
        ChangeStateTo(following_state);
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
        if(!stopping_phase)
        {
            if (returning)
            {
                if (current_angle > right_zone)
                    current_speed = Mathf.Lerp(current_speed, max_rotation_speed, 0.02f);
                else if (current_angle < left_zone)
                    current_speed = Mathf.Lerp(current_speed, min_rotation_speed, 0.02f);

                current_angle -= current_speed * Time.deltaTime;
                camera_lens.transform.eulerAngles += new Vector3(0.0f, -current_speed * Time.deltaTime, 0.0f);
            }
            else
            {
                if (current_angle > right_zone)
                    current_speed = Mathf.Lerp(current_speed, min_rotation_speed, 0.02f);
                else if (current_angle < left_zone)
                    current_speed = Mathf.Lerp(current_speed, max_rotation_speed, 0.02f);

                current_angle += current_speed * Time.deltaTime;
                camera_lens.transform.eulerAngles += new Vector3(0.0f, current_speed * Time.deltaTime, 0.0f);
            }
        }        

        if (returning && current_angle < 1.0f || !returning && Mathf.Abs(current_angle - angle_range) < 1.0f)
        {
            stopping_phase = true;
            if (timer > delay_time_sweep)
            {
                returning = !returning;
                timer = 0.0f;
                stopping_phase = false;
            }
            else
                timer += Time.deltaTime;
        }
            
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


