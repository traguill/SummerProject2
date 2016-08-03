using UnityEngine;
using System.Collections;
using System;

public class BarionMovingBoxState : IBarionState {

    private readonly BarionController barion;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    GameObject box; //Object to move

    float carry_box_distance = 6; //Distance from Barion to box to carry. 
    float box_height = 4; //Height of the box when is carried.

    bool moving_to_box; //Is going to pick up the box?
    bool carrying_box; //Is carrying the box while moving?

    public bool drop_box; //Want to drop the box?

    public BarionMovingBoxState(BarionController barion_controller)
    {
        barion = barion_controller;
        box = null;
    }

    public void StartState()
    {
        //First time in this state
        if (barion.target_box != null)
        {
            box = barion.target_box; //Save box info
            barion.target_box = null; //Reset controller input

            moving_to_box = false; //Reset variables
            carrying_box = false;
            drop_box = false;

            if (Vector3.Distance(barion.transform.position, box.transform.position) > carry_box_distance) //Too far to pick up the box
            {
                barion.agent.SetDestination(box.transform.position); //Go to box position
                moving_to_box = true;
            }
            else
            {
                PickUpBox();
            }
        }
    }

    public void UpdateState()
    {
        //TODO: cancel actions in progress

       

       //The box is far, Barion is approaching
       if(moving_to_box == true)
        {
            if(Vector3.Distance(barion.transform.position, box.transform.position) <= barion.agent.stoppingDistance)
            {
                barion.StopMovement();
                PickUpBox();
            }
            else
            {
                barion.agent.SetDestination(box.transform.position); //Constantly follow the box
            }
        }

       //Is carrying the box, now move with it.
       if(carrying_box == true)
       {
            //Get new destination
            if(barion.is_selected)
            {
                if (barion.GetMovement(ref destination))
                {
                    barion.agent.SetDestination(destination);
                }
            }

            //Update box position
            Vector3 pos = barion.transform.position;
            box.transform.position = new Vector3(pos.x, box_height, pos.z);

            if(drop_box == true)
            {
                DropBox();
            }
        }
    }

    public void ToIdleState()
    {
        barion.ChangeStateTo(barion.idle_state);
        barion.StopMovement();
    }

    public void ToMoveBoxState()
    {
       //TODO
    }

    public void ToWalkingState()
    {
        barion.ChangeStateTo(barion.walking_state);
        barion.agent.SetDestination(destination);
    }

    public void ToHideState()
    {
        Debug.Log("Barion can't transition from MOVING_BOX to HIDING");
    }

    public void ToInvisibleSphereState()
    {
        Debug.Log("Barion can't transition from HIDING to SPHERE");
    }

    public void ToShieldState()
    {
        Debug.Log("Barion can't transition from MOVING_BOX to SHIELD");
    }

    /// <summary>
    /// Updates the state depending on the current conditions of each state.
    /// </summary>
    void Transitions()
    {
        //Can transition to walking
        if (barion.GetMovement(ref destination))
        {
            ToWalkingState();
        }

        //Multiple box or diferent box option (TODO)
    }

    /// <summary>
    /// Handles all the logic and animation to the action to pick up the box.
    /// </summary>
    void PickUpBox()
    {
        moving_to_box = false;
        carrying_box = true;
        Vector3 pos = barion.transform.position;
        box.transform.position = new Vector3(pos.x, box_height, pos.z);
    }

    /// <summary>
    /// Returns if Barion is currently carrying the box.
    /// </summary>
    public bool IsCarryingBox()
    {
        return carrying_box;
    }
    
    /// <summary>
    /// Handles the action to drop the box to the floor
    /// </summary>
    void DropBox()
    {
        //For now it drops the box to the right
        Vector3 pos = barion.transform.position;
        box.transform.position = new Vector3(pos.x + carry_box_distance, 1, pos.z);

        ToIdleState();
    }



}
