using UnityEngine;
using System.Collections;

public class CameraFollowingState : ICameraStates
{
    private readonly CameraController camera;

    GameObject player;

    public CameraFollowingState(CameraController camera_controller)
    {
        camera = camera_controller;
    }

    public void StartState()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
    }

    public void UpdateState()
    {
        camera.camera_lens.LookAt(player.transform);
    }

    public void ToIdleState()
    {
        camera.ChangeStateTo(camera.idle_state);
    }

    public void ToAlertState()
    {
        camera.ChangeStateTo(camera.alert_state);
    }

    public void ToFollowingState()
    {
        Debug.Log("Camera" + camera.name + "can't transition to same state FOLLOWING");
    }
}
