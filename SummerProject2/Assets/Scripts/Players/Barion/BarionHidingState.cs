using UnityEngine;
using System.Collections;
using System;

public class BarionHidingState : IBarionState
{
    private readonly BarionController barion;

    bool hiding; //Is the player hidded.

    float hide_distance = 4; //Distance from player to the box that will automatically hide.

    GameObject box;

    Vector3 destination = new Vector3(); //Pathfidning destination

    float box_spawn_delay = 3; //Delay in Z axis when getting out the box.

    public BarionHidingState(BarionController barion_controller)
    {
        barion = barion_controller;
    }

    public void StartState()
    {
        hiding = false;

        box = barion.target_box;

        if (Vector3.Distance(barion.transform.position, box.transform.position) > hide_distance)
        { //Go to the box
            barion.agent.SetDestination(box.transform.position);
        }
        else
        { //Hide
            Hide();  
        }
    }

    public void UpdateState()
    {
        //Check arrive at box
        if(hiding == false)
        {
            if(barion.DstArrived())
            {
                Hide();
            }
        }
        else
        { //Actually hiding

            if (barion.is_selected)
            {
                if(barion.GetMovement(ref destination)) //Get movement input
                {
                    GetOut(); //Get out the box
                }
            }
        }
    }

    public void ToHideState()
    {
    }

    public void ToIdleState()
    {
        Debug.Log("Barion can't transition from HIDING to IDLE");
    }

    public void ToMoveBoxState()
    {
        Debug.Log("Barion can't transition from HIDING to MOVE_BOX");
    }

    public void ToWalkingState()
    {
        barion.ChangeStateTo(barion.walking_state);
    }

    public void ToInvisibleSphereState()
    {
        Debug.Log("Barion can't transition from HIDING to SPHERE");
    }

    void Hide()
    {
        hiding = true;
        barion.StopMovement();

        barion.transform.position = new Vector3(box.transform.position.x, barion.transform.position.y, box.transform.position.z); //Set same pos as box.
        barion.is_hide = true;
        barion.selection_system.DeselectPlayer(barion.gameObject); //Deselect the player
        barion.GetComponent<SpriteRenderer>().enabled = false; //Hide sprite
        barion.GetComponent<NavMeshAgent>().enabled = false; //Disable the nav mesh to disable collisions

        box.GetComponent<BoxController>().HideCharacter(HideTags.barion); //Display UI of hide character
    }

    void GetOut()
    {
        hiding = false;
        barion.transform.position = new Vector3(box.transform.position.x, barion.transform.position.y, box.transform.position.z - box_spawn_delay); //Set position in front
        barion.is_hide = false;
        barion.GetComponent<SpriteRenderer>().enabled = true; //Show sprite
        barion.GetComponent<NavMeshAgent>().enabled = true; //Enable nav mesh to enable collisions
        barion.agent.SetDestination(destination); //Set new destination


        box.GetComponent<BoxController>().RemoveHideCharacter(HideTags.barion);

        barion.target_box = null; //No box related

        ToWalkingState(); //Change state

    }

    
}
