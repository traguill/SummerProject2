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
        
    }

    public void UpdateState()
    {
        
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
