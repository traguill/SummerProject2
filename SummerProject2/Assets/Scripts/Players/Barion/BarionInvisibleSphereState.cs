using UnityEngine;
using System.Collections;
using System;

public class BarionInvisibleSphereState : IBarionState
{
    private readonly BarionController barion;

    public BarionInvisibleSphereState(BarionController barion_controller)
    {
        barion = barion_controller;
    }
    public void StartState()
    {
        barion.StopMovement(); //Stop

        //Get throw direction
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
            Debug.Log("ERROR: Barion can't use INVISIBLE SPHERE because the mouse didn't find a floor collison to set the direction");
        }

        //Create invisible sphere
        GameObject sphere = GameObject.Instantiate(barion.invisible_sphere_prefab, barion.transform.position, new Quaternion()) as GameObject;

        sphere.GetComponent<InvisibleSphere>().direction = direction;
        

        barion.cooldown_inst.StartCooldown(1); //Start cooldown of ability1
    }

    public void UpdateState()
    {
        //TODO: the ability should lasts the same as the animation.
        ToIdleState();
    }

    public void ToHideState()
    {
        Debug.Log("Barion can't transition from SPHERE to HIDE");
    }

    public void ToIdleState()
    {
        barion.ChangeStateTo(barion.idle_state);
    }

    public void ToInvisibleSphereState()
    {
        Debug.Log("Barion can't transition from SPHERE to SPHERE");
    }

    public void ToMoveBoxState()
    {
        Debug.Log("Barion can't transition from SPHERE to MOVE_BOX");
    }

    public void ToWalkingState()
    {
        Debug.Log("Barion can't transition from SPHERE to WALKING");
    }

    
}
