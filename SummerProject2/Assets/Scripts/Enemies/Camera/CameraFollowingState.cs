using UnityEngine;
using System.Collections;

public class CameraFollowingState : ICameraStates
{
    private readonly CameraController camera;
    private float time_without_detection;
    private GameObject player_to_follow;

    public CameraFollowingState(CameraController camera_controller)
    {
        camera = camera_controller;
    }

    public void StartState()
    {
        time_without_detection = 0.0f;
        player_to_follow = NearPlayer();
    }

    public void UpdateState()
    {
        FollowNearPlayer();
        IsTimeExceeded();      
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

    private void FollowNearPlayer()
    {
        float angle = Vector3.Angle(camera.initial_forward_direction, (player_to_follow.transform.position - camera.camera_lens.transform.position).normalized);

        if( angle < camera.mid_angle)
        {
            camera.camera_lens.LookAt(player_to_follow.transform);
            time_without_detection = 0.0f;
        }            
        else
        {
            time_without_detection += Time.deltaTime;
        }

        Debug.DrawLine(camera.camera_lens.position, player_to_follow.transform.position);
        //Debug.Log(Vector3.Distance(camera.camera_lens.position, player_to_follow.transform.position));
    }

    private GameObject NearPlayer()
    {
        GameObject near_player = null;
        float min_distance = 1000.0f;

        foreach (GameObject p in camera.players)
        {
            if (Vector3.Distance(camera.transform.position, p.transform.position) < min_distance)
            {
                min_distance = Vector3.Distance(camera.transform.position, p.transform.position);
                near_player = p;
            }
        }

        return near_player;
    }

    private void IsTimeExceeded()
    {
        // CRZ TODO: Alert mode is not implemented!
        if (time_without_detection > camera.seconds_from_last_sight)
        {
            if (camera.alarm_system.isAlarmActive())
                camera.ChangeStateTo(camera.idle_state);
            else
                camera.ChangeStateTo(camera.idle_state);
        }            
    }

}
