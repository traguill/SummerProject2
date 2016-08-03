﻿using UnityEngine;
using System.Collections;
using System;

public class BarionShieldState : IBarionState
{
    private readonly BarionController barion;

    public float spawn_shield_radius = 3;

    public BarionShieldState(BarionController barion_controller)
    {
        barion = barion_controller;
    }

    public void StartState()
    {
        barion.cooldown_inst.StartCooldown(2);

        //Create shield

        //Calculate direction with mouse
        Vector3 direction = new Vector3();

        Ray ray_mouse = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray to mouse position to set direction of the sphere
        RaycastHit hit_mouse;
        if (Physics.Raycast(ray_mouse, out hit_mouse, 100, barion.floor_layer))
        {
            direction = hit_mouse.point;
            direction.y = barion.transform.position.y; //Same height
            direction = direction - barion.transform.position;
            direction.Normalize();
        }
        else
        {
            Debug.Log("ERROR: Barion can't use SHIELD because the mouse didn't find a floor collison to set the direction");
        }

        Vector3 shield_position = barion.transform.position + (direction * spawn_shield_radius); //Position to create the shield

        //Calculate the angle of rotation of the shield

        Quaternion rot_angle =  Quaternion.FromToRotation(Vector3.right, direction);

        //Create SHIELD
        GameObject shield = GameObject.Instantiate(barion.shield_prefab, shield_position, rot_angle) as GameObject;

        shield.GetComponent<InvisibleShield>().barion = barion;
        shield.GetComponent<InvisibleShield>().delay_pos = shield_position - barion.transform.position;

        if(barion.agent.hasPath)
        {
            ToWalkingState(); //Continue walking
        }
        else
        {
            ToIdleState(); 
        }
    }

    public void UpdateState()
    {
       //Insta ability. No update for now
    }

    public void ToHideState()
    {
        Debug.Log("Barion can't transition form SHIELD to HIDING");
    }

    public void ToIdleState()
    {
        barion.ChangeStateTo(barion.idle_state);
    }

    public void ToInvisibleSphereState()
    {
        Debug.Log("Barion can't transition form SHIELD to INVISIBLE_SPHERE");
    }

    public void ToMoveBoxState()
    {
        Debug.Log("Barion can't transition form SHIELD to MOVE_BOX");
    }

    public void ToShieldState()
    {
        Debug.Log("Barion can't transition form SHIELD to SHIELD");
    }

    public void ToWalkingState()
    {
        barion.ChangeStateTo(barion.walking_state);
    }

    
}