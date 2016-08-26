using UnityEngine;
using System.Collections;

public class BarionCarryCorpseState : IBarionState 
{
    private readonly BarionController barion;

    private Vector3 destination = new Vector3(); //Pathfinding destination.

    GameObject corpse; //Corpse to move

    float carry_corpse_distance = 6; //Distance from Barion to corpse to carry. 
    float corpse_height = 4; //Height of the corpse when is carried.

    bool moving_to_corpse; //Is going to pick up the corpse?
    bool carrying_corpse; //Is carrying the corpse while moving?

    public bool drop_corpse; //Want to drop the corpse?

    public  BarionCarryCorpseState(BarionController barion_controller)
    {
        barion = barion_controller;
        corpse = null;
    }

    public void StartState()
    {
        //First time in this state
        if (barion.target_corpse != null)
        {
            corpse = barion.target_corpse; //Save corpse info
            barion.target_corpse = null; //Reset controller input

            moving_to_corpse = false; //Reset variables
            carrying_corpse = false;
            drop_corpse = false;

            if (Vector3.Distance(barion.transform.position, corpse.transform.position) > carry_corpse_distance) //Too far to pick up the corpse
            {
                barion.agent.SetDestination(corpse.transform.position); //Go to corpse position
                moving_to_corpse = true;
            }
            else
            {
                PickUpCorpse();
            }
        }
    }

    public void UpdateState()
    {
        //TODO: cancel actions in progress



        //The corpse is far, Barion is approaching
        if (moving_to_corpse == true)
        {
            if (Vector3.Distance(barion.transform.position, corpse.transform.position) <= barion.agent.stoppingDistance)
            {
                barion.StopMovement();
                PickUpCorpse();
            }
            else
            {
                barion.agent.SetDestination(corpse.transform.position); //Constantly follow the corpse //TODO: Double check why I do this. The box isn't going anywhere, why set the destination every update?
            }
        }

        //Is carrying the corpse, now move with it.
        if (carrying_corpse == true)
        {
            //Get new destination
            if (barion.is_selected)
            {
                if (barion.GetMovement(ref destination))
                {
                    barion.agent.SetDestination(destination);
                }
            }

            //Update corpse position
            Vector3 pos = barion.transform.position;
            corpse.transform.position = new Vector3(pos.x, corpse_height, pos.z);

            if (drop_corpse == true)
            {
                DropCorpse();
            }
        }
    }

    public void ToIdleState()
    {
        barion.ChangeStateTo(barion.idle_state);
        barion.StopMovement();
    }

    public void ToWalkingState()
    {
        barion.agent.SetDestination(destination);
        barion.ChangeStateTo(barion.walking_state);       
    }

    public void ToMoveBoxState()
    {
        Debug.Log("Barion can't transition from CARRY_CORPSE to MOVE_BOX");
    }

    public void ToHideState()
    {
        Debug.Log("Barion can't transition from CARRY_CORPSE to HIDE");
    }

    public void ToInvisibleSphereState()
    {
        Debug.Log("Barion can't transition from CARRY_CORPSE to INVISIBLE_SPHERE");
    }

    public void ToShieldState()
    {
        Debug.Log("Barion can't transition from CARRY_CORPSE to SHIELD");
    }

    public void ToCarryCorpseState()
    {
       //TODO: 
    }




    /// <summary>
    /// Handles all the logic and animation to the action to pick up the corpse.
    /// </summary>
    void PickUpCorpse()
    {
        moving_to_corpse = false;
        carrying_corpse = true;
        Vector3 pos = barion.transform.position;
        corpse.transform.position = new Vector3(pos.x, corpse_height, pos.z);
    }

    /// <summary>
    /// Returns if Barion is currently carrying the corpse.
    /// </summary>
    public bool IsCarryingCorpse()
    {
        return carrying_corpse;
    }

    /// <summary>
    /// Handles the action to drop the corpse to the floor
    /// </summary>
    void DropCorpse()
    {
        //For now it drops the box to the right
        Vector3 pos = barion.transform.position;
        Vector3 direction = barion.agent.velocity.normalized;

        corpse.transform.position = pos + (direction * carry_corpse_distance);

        ToIdleState();
    }
}
