using UnityEngine;
using System.Collections;
using System;

public class CosmoHidingState : ICosmoState
{
    private readonly CosmoController cosmo;

    bool hiding; //Is the player hidded.

    float hide_distance = 4; //Distance from player to the box that will automatically hide.

    GameObject box;

    Vector3 destination = new Vector3(); //Pathfidning destination

    float box_spawn_delay = 3; //Delay in Z axis when getting out the box.

    public CosmoHidingState(CosmoController cosmo_controller)
    {
        cosmo = cosmo_controller;
    }

    public void StartState()
    {
        hiding = false;

        box = cosmo.target_box;

        if (Vector3.Distance(cosmo.transform.position, box.transform.position) > hide_distance)
        { //Go to the box
            cosmo.agent.SetDestination(box.transform.position);
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
            if (cosmo.DstArrived())
            {
                Hide();
            }
        }
        else
        { //Actually hiding

            if (cosmo.is_selected)
            {
                if (cosmo.GetMovement(ref destination)) //Get movement input
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
        Debug.Log("Cosmo can't transition from HIDING to IDLE");
    }

    public void ToSensorialState()
    {
        Debug.Log("Cosmo can't transition from HIDING to SENSORIAL");
    }

    public void ToWalkingState()
    {
        cosmo.ChangeStateTo(cosmo.walking_state);
    }

    void Hide()
    {
        hiding = true;
        cosmo.StopMovement();

        cosmo.transform.position = new Vector3(box.transform.position.x, cosmo.transform.position.y, box.transform.position.z); //Set same pos as box.
        cosmo.is_hide = true;
        cosmo.selection_system.DeselectPlayer(cosmo.gameObject); //Deselect the player
        cosmo.GetComponent<SpriteRenderer>().enabled = false; //Hide sprite
        cosmo.GetComponent<NavMeshAgent>().enabled = false; //Disable the nav mesh to disable collisions
        box.GetComponent<BoxController>().HideCharacter(HideTags.cosmo); //Display UI of hide character
    }

    void GetOut()
    {
        hiding = false;
        cosmo.transform.position = new Vector3(box.transform.position.x, cosmo.transform.position.y, box.transform.position.z - box_spawn_delay); //Set position in front
        cosmo.is_hide = false;
        cosmo.GetComponent<SpriteRenderer>().enabled = true; //Show sprite
        cosmo.GetComponent<NavMeshAgent>().enabled = true; //Enable nav mesh to enable collisions
        cosmo.agent.SetDestination(destination); //Set new destination
        
        box.GetComponent<BoxController>().RemoveHideCharacter(HideTags.cosmo);

        cosmo.target_box = null; //No box related

        ToWalkingState(); //Change state

    }
}
