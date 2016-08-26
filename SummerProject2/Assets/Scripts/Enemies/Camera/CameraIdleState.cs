using UnityEngine;
using System.Collections;

public class CameraIdleState : ICameraStates {

    private readonly CameraController camera;
    private bool initial_position_recovered;                        // Determines whether the camera can start its sweep or not.
    private float time_recovering_timer, max_time_recovering;       // Time use by the camera to recover its standart orientation.
   
    public CameraIdleState(CameraController camera_controller)
    {
        camera = camera_controller;
    }

    public void StartState()
    {
        camera.current_angle = Vector3.Angle(camera.initial_forward_direction, camera.camera_lens.forward);
        ANGLE_DIR angle_dir = camera.AngleDir(camera.initial_forward_direction, camera.camera_lens.forward, camera.camera_lens.up);

        switch(angle_dir)
        {
            case (ANGLE_DIR.FORWARD):
                camera.current_angle = camera.mid_angle;
                break;
            case (ANGLE_DIR.LEFT):
                camera.current_angle = camera.mid_angle - camera.current_angle;
                break;
            case (ANGLE_DIR.RIGHT):
                camera.current_angle += camera.mid_angle;
                break;
        }

        initial_position_recovered = false;         
        max_time_recovering = time_recovering_timer = 1.0f; 
    }

    public void UpdateState()
    {
        if (!initial_position_recovered)
            RecoveringPosition();
        else if(!camera.static_camera)
            camera.CameraSweep();
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

    private void RecoveringPosition()
    {
        Vector3 initial_dir = camera.initial_rotation.eulerAngles;
        Vector3 current_dir = camera.camera_lens.rotation.eulerAngles;   

        float speed_x = (initial_dir.x - current_dir.x) / time_recovering_timer;
        float speed_z = (initial_dir.z - current_dir.z) / time_recovering_timer;

        camera.camera_lens.transform.eulerAngles += new Vector3(speed_x * Time.deltaTime, 0.0f, speed_z * Time.deltaTime);
        time_recovering_timer -= Time.deltaTime;

        current_dir = camera.camera_lens.rotation.eulerAngles; // Updating values

        if (Mathf.Abs(initial_dir.x - current_dir.x) < 0.1f && Mathf.Abs(initial_dir.z - current_dir.z) < 0.1f)
        {
            initial_position_recovered = true;
            time_recovering_timer = max_time_recovering;
        }
    }
}

