using UnityEngine;
using System.Collections;

public class CameraIdleState : ICameraStates {

    private readonly CameraController camera;
    private bool returning;
    private float current_angle, forward_angle, current_speed, min_speed;

    public CameraIdleState(CameraController camera_controller)
    {
        camera = camera_controller;
    }

    public void StartState()
    {
        returning = false;
        current_angle = forward_angle = camera.angle_range / 2.0f;
        current_speed = camera.max_rotation_speed;
    }

    public void UpdateState()
    {
        if(returning)
        {
            if (current_angle < forward_angle)
                current_speed = Mathf.Lerp(current_speed, camera.min_rotation_speed, 0.02f);            
            else
                current_speed = Mathf.Lerp(current_speed, camera.max_rotation_speed, 0.02f);

            current_angle -= current_speed * Time.deltaTime;
            camera.camera_lens.transform.eulerAngles += new Vector3(0.0f, -current_speed * Time.deltaTime, 0.0f);
        }
        else
        {
            if (current_angle > forward_angle)
                current_speed = Mathf.Lerp(current_speed, camera.min_rotation_speed, 0.02f);
            else
                current_speed = Mathf.Lerp(current_speed, camera.max_rotation_speed, 0.02f);

            current_angle += current_speed * Time.deltaTime;
            camera.camera_lens.transform.eulerAngles += new Vector3(0.0f, current_speed * Time.deltaTime, 0.0f);
        }

        if (returning && current_angle < 1.0f || !returning && Mathf.Abs(current_angle - camera.angle_range) < 1.0f)
            returning = !returning;

        Debug.Log(current_speed);
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
