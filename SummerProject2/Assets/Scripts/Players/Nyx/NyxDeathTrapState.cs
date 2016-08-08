using UnityEngine;
using System.Collections;

public class NyxDeathTrapState : INyxState 
{
    private readonly NyxController nyx;

    public DeathTrap trap = null; //Reference to the current trap

    public NyxDeathTrapState(NyxController nyx_controller)
    {
        nyx = nyx_controller;
    }

    public void StartState()
    {
        nyx.cooldown_inst.StartCooldown(2);

        //Destroy previous trap if exists
        if(trap != null)
        {
              GameObject.Destroy(trap.gameObject);
        }

        //Get cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, nyx.floor_layer))
        {
            Vector3 trap_pos = new Vector3(hit.point.x, 0, hit.point.z); //Death trap has 0 in Y axis, is on the floor.
            Vector3 nyx_pos = nyx.transform.position;

            //Check range
            if(Vector3.Distance(nyx_pos, trap_pos) > nyx.death_trap_range)
            {
                Vector3 direction = trap_pos - nyx_pos;
                direction.Normalize();

                trap_pos = nyx_pos + (direction * nyx.death_trap_range);
            }

            //Check available zone to put the trap (TODO)

            //Create the game object
            GameObject new_trap = GameObject.Instantiate(nyx.death_trap_prefab, trap_pos, new Quaternion()) as GameObject;
            trap = new_trap.GetComponent<DeathTrap>();

            trap.nyx_death_trap_state = this;
            
        }
        else
        {
            Debug.Log("ERROR: Nyx can't create a death trap because the mouse didn't hit the floor");
        }

       

       
    }

    public void UpdateState()
    {
       //Instant-> change state to idle

        ToIdleState();
    }

    public void ToIdleState()
    {
        nyx.ChangeStateTo(nyx.idle_state);
    }

    public void ToWalkingState()
    {
        Debug.Log("Nyx can't transition from DEATH_TRAP to WALKING");
    }

    public void ToKillingState()
    {
        Debug.Log("Nyx can't transition from DEATH_TRAP to KILLING");
    }

    public void ToHideState()
    {
        Debug.Log("Nyx can't transition from DEATH_TRAP to HIDE");
    }

    public void ToDashState()
    {
        Debug.Log("Nyx can't transition from DEATH_TRAP to DASH");
    }

    public void ToDeathTrapState()
    {
        Debug.Log("Nyx can't transition from DEATH_TRAP to DEATH_TRAP");
    }
}
