using UnityEngine;
using System.Collections;

public class CameraIdleState : ICameraStates {

    private readonly CameraController camera;
    private bool returning;
    private float current_speed;
    private float current_angle;

    public CameraIdleState(CameraController camera_controller)
    {
        camera = camera_controller;
    }

    public void StartState()
    {
        returning = false;
        current_speed = 15.0f;
        current_angle = 30.0f;
    }

    public void UpdateState()
    {
        float max_angle, min_angle;
        max_angle = camera.max_angle;
        min_angle = -camera.max_angle;
        float min_speed, max_speed;
        max_speed = 15.0f;
        min_speed = 2.0f;

        if(returning)
        {
            if (current_angle < max_angle)
                current_speed = Mathf.Lerp(current_speed, min_speed, 0.05f);            
            else
                current_speed = Mathf.Lerp(current_speed, max_speed, 0.05f);

            current_angle -= current_speed * Time.deltaTime;
            camera.camera_lens.transform.eulerAngles += new Vector3(0.0f, -current_speed * Time.deltaTime, 0.0f);
        }
        else
        {
            if (current_angle > max_angle)
                current_speed = Mathf.Lerp(current_speed, min_speed, 0.05f);
            else
                current_speed = Mathf.Lerp(current_speed, max_speed, 0.05f);

            current_angle += current_speed * Time.deltaTime;
            camera.camera_lens.transform.eulerAngles += new Vector3(0.0f, current_speed * Time.deltaTime, 0.0f);
        }

        if (returning && current_angle < 1.0f || !returning && Mathf.Abs(current_angle - (2.0f * max_angle)) < 1.0f)
            returning = !returning;
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
