using UnityEngine;
using System.Collections;

public class CameraIdleState : ICameraStates {

    private readonly CameraController camera;
    private bool returning = false;                           // Useful to separate each camera direction
    private float current_angle, current_speed, min_speed;

    public CameraIdleState(CameraController camera_controller)
    {
        camera = camera_controller;
        current_angle = camera.mid_angle = camera.angle_range / 2.0f;
        current_speed = camera.max_rotation_speed;
    }

    public void StartState()
    {
        current_angle = Vector3.Angle(camera.initial_forward_direction, camera.camera_lens.forward);
        ANGLE_DIR angle_dir = AngleDir(camera.initial_forward_direction, camera.camera_lens.forward, camera.camera_lens.up);

        switch(angle_dir)
        {
            case (ANGLE_DIR.FORWARD):
                current_angle = camera.mid_angle;
                break;
            case (ANGLE_DIR.LEFT):
                current_angle = camera.mid_angle - current_angle;
                break;
            case (ANGLE_DIR.RIGHT):
                current_angle += camera.mid_angle;
                break;
        }        
    }

    public void UpdateState()
    {
        if (!camera.static_camera)
            CameraSweep();
    }

    public void ToIdleState()
    {
        Debug.Log("Camera" + camera.name + "can't transition to same state IDLE");
    }

    public void ToAlertState()
    {
        camera.ChangeStateTo(camera.alert_state);
    }

    public void ToFollowingState()
    {
        camera.ChangeStateTo(camera.following_state);
    }

    private void CameraSweep()
    {
        if (returning)
        {
            if (current_angle < camera.mid_angle)
                current_speed = Mathf.Lerp(current_speed, camera.min_rotation_speed, 0.02f);
            else
                current_speed = Mathf.Lerp(current_speed, camera.max_rotation_speed, 0.02f);

            current_angle -= current_speed * Time.deltaTime;
            camera.camera_lens.transform.eulerAngles += new Vector3(0.0f, -current_speed * Time.deltaTime, 0.0f);
        }
        else
        {
            if (current_angle > camera.mid_angle)
                current_speed = Mathf.Lerp(current_speed, camera.min_rotation_speed, 0.02f);
            else
                current_speed = Mathf.Lerp(current_speed, camera.max_rotation_speed, 0.02f);

            current_angle += current_speed * Time.deltaTime;
            camera.camera_lens.transform.eulerAngles += new Vector3(0.0f, current_speed * Time.deltaTime, 0.0f);
        }

        if (returning && current_angle < 1.0f || !returning && Mathf.Abs(current_angle - camera.angle_range) < 1.0f)
            returning = !returning;

        Vector2 from, to;
        from = new Vector2(camera.initial_forward_direction.x, camera.initial_forward_direction.z);
        to = new Vector2(camera.camera_lens.forward.x, camera.camera_lens.forward.z);
        Debug.Log(Vector2.Angle(from, to));
        

    }

    enum ANGLE_DIR
    {
        FORWARD,
        RIGHT,
        LEFT
    };

    ANGLE_DIR AngleDir(Vector3 fwd,  Vector3 target_direction, Vector3 up)
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
