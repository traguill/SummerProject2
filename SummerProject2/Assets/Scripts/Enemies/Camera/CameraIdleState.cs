using UnityEngine;
using System.Collections;

public class CameraIdleState : ICameraStates {

    private readonly CameraController camera;
   
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
    }

    public void UpdateState()
    {
        if (!camera.static_camera)
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
}
