using UnityEngine;
using System.Collections;
using System;

public class NyxDashState : INyxState {

    private readonly NyxController nyx;

    public Vector3 destination = new Vector3(); //Final position of the dash
    Vector3 direction = new Vector3(); //Direction of nyx dashing

    public NyxDashState(NyxController nyx_controller)
    {
        nyx = nyx_controller;
    }

    public void StartState()
    {
        nyx.cooldown_inst.StartCooldown(1); //Starts the cooldown

        nyx.StopMovement(); //Stops the movement

        nyx.agent.enabled = false; //Disable navmesh agent collisions
        
        Ray ray_mouse = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray to mouse position to set destination of the dash
        RaycastHit hit_mouse;
        if(Physics.Raycast(ray_mouse, out hit_mouse, 100, nyx.floor_layer))
        {
            destination = hit_mouse.point;
            destination.y = nyx.transform.position.y;

            //Calculate nyx direction to destination
            direction = destination - nyx.transform.position;
            direction.Normalize();

            //Destination point in range
            float distance = Vector3.Distance(nyx.transform.position, destination);
            if(distance > nyx.dash_range)
            { //Calculate new destination point
                Vector3 new_destination = direction * nyx.dash_range;
                destination = nyx.transform.position + new_destination;

                distance = Vector3.Distance(nyx.transform.position, destination); //Update distance
            }

            //Check if something is between the desired destination point and nyx
            Ray ray_dash = new Ray(nyx.transform.position, direction);
            RaycastHit hit_dash;
            if(Physics.Raycast(ray_dash, out hit_dash, distance, nyx.dash_collision_layer))
            {
                if(hit_dash.transform.tag == Tags.enemy) //Enemy found in the way
                {
                    nyx.enemy_manager.DestroyEnemy(hit_dash.transform.gameObject); //Kill enemy in the way 
                }
                else
                {
                    destination = hit_dash.point; //Something is between nyx and the dash final position
                }
                
            }

            
        }
        else
        {
            Debug.Log("ERROR: Nyx can't find a floor point to start dashing");
        }
    }

    public void UpdateState()
    {
        //Update position
        nyx.transform.position += nyx.dash_speed * Time.deltaTime * direction;

        //Check arrive destination
        Vector3 dst = destination - nyx.transform.position;
        if(Mathf.Sign(dst.x) != Mathf.Sign(direction.x) || Mathf.Sign(dst.z) != Mathf.Sign(direction.z))
        {
            nyx.transform.position = new Vector3(destination.x, nyx.transform.position.y, destination.z);
            ToIdleState();
        }
    }

    public void ToDashState()
    {
        Debug.Log("Nyx can't transition from DASH to DASH");
    }

    public void ToHideState()
    {
        Debug.Log("Nyx can't transition from DASH to HIDE");
    }

    public void ToIdleState()
    {
        nyx.agent.enabled = true; //Enable navmesh agent collisions
        nyx.ChangeStateTo(nyx.idle_state);
    }

    public void ToKillingState()
    {
        Debug.Log("Nyx can't transition from DASH to KILLING");
    }

    public void ToWalkingState()
    {
        Debug.Log("Nyx can't transition from DASH to WALKING");
    }


    public void ToDeathTrapState()
    {
        Debug.Log("Nyx can't transition from DASH to DEATH_TRAP");
    }

    public void ToChainedState()
    {
        Debug.Log("Nyx can't transition from WALKING to CHAINED");
    }
   
}
