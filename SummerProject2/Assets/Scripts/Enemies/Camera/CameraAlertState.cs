using UnityEngine;
using System.Collections;

public class CameraAlertState : ICameraStates
{
    private readonly CameraController camera;

    public CameraAlertState(CameraController camera_controller)
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
        camera.ChangeStateTo(camera.idle_state);
    }

    public void ToAlertState()
    {
        Debug.Log("Camera" + camera.name + "can't transition to the same state ALERT");
    }

    public void ToFollowingState()
    {
        camera.ChangeStateTo(camera.following_state);
    }
}
