using UnityEngine;
using System.Collections;
using System;

public class NyxHidingState : INyxState
{
    private readonly NyxController nyx;

    bool hiding; //Is the player hidded.

    float hide_distance = 4; //Distance from player to the box that will automatically hide.

    GameObject box;

    Vector3 destination = new Vector3(); //Pathfidning destination

    float box_spawn_delay = 3; //Delay in Z axis when getting out the box.

    public NyxHidingState(NyxController nyx_controller)
    {
        nyx = nyx_controller;
    }

    public void StartState()
    {
        hiding = false;

        box = nyx.target_box;

        if (Vector3.Distance(nyx.transform.position, box.transform.position) > hide_distance)
        { //Go to the box
            nyx.agent.SetDestination(box.transform.position);
        }
        else
        { //Hide
            Hide();
        }
    }

    public void UpdateState()
    {
        //Check arrive at box
        if (hiding == false)
        {
            if (nyx.DstArrived())
            {
                Hide();
            }
        }
        else
        { //Actually hiding

            if (nyx.is_selected)
            {
                if (nyx.GetMovement(ref destination)) //Get movement input
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
        Debug.Log("Nyx can't transition from HIDING to IDLE");
    }

    public void ToKillingState()
    {
        Debug.Log("Nyx can't transition from HIDING to KILLING");
    }

    public void ToWalkingState()
    {
        nyx.ChangeStateTo(nyx.walking_state);
    }

    public void ToDashState()
    {
        Debug.Log("Nyx can't transition form HIDING to DASH");
    }

    void Hide()
    {
        hiding = true;
        nyx.StopMovement();

        nyx.transform.position = new Vector3(box.transform.position.x, nyx.transform.position.y, box.transform.position.z); //Set same pos as box.
        nyx.is_hide = true;
        nyx.selection_system.DeselectPlayer(nyx.gameObject); //Deselect the player
        nyx.GetComponent<SpriteRenderer>().enabled = false; //Hide sprite
        nyx.GetComponent<NavMeshAgent>().enabled = false; //Disable the nav mesh to disable collisions

        box.GetComponent<BoxController>().HideCharacter(HideTags.nyx); //Display UI of hide character
    }

    void GetOut()
    {
        hiding = false;
        nyx.transform.position = new Vector3(box.transform.position.x, nyx.transform.position.y, box.transform.position.z - box_spawn_delay); //Set position in front
        nyx.is_hide = false;
        nyx.GetComponent<SpriteRenderer>().enabled = true; //Show sprite
        nyx.GetComponent<NavMeshAgent>().enabled = true; //Enable nav mesh to enable collisions
        nyx.agent.SetDestination(destination); //Set new destination


        box.GetComponent<BoxController>().RemoveHideCharacter(HideTags.nyx);

        nyx.target_box = null; //No box related

        ToWalkingState(); //Change state

    }

    
}
