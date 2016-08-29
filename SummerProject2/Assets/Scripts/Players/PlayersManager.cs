using UnityEngine;
using System.Collections;

public class PlayersManager : MonoBehaviour
{
    public LayerMask environment_collision_layer;

    //Players controller
    CosmoController cosmo;
    NyxController nyx;
    BarionController barion;

    //Reference to other scripts
    [HideInInspector]public CursorManager cursor; 

    void Awake()
    {
        cosmo = GameObject.Find("Cosmo").GetComponent<CosmoController>();
        nyx = GameObject.Find("Nyx").GetComponent<NyxController>();
        barion = GameObject.Find("Barion").GetComponent<BarionController>();

        cursor = GameObject.Find("Cursor").GetComponent<CursorManager>();
    }

	
    /// <summary>
    /// Sets the new destination for the player and handles all the conficts that could happen between states.
    /// </summary>
    public void CrossPortal(GameObject player, Vector3 second_portal_position)
    {
        //Cosmo
        if(player.name == "Cosmo")
        {
            Teleport(cosmo.gameObject, cosmo.agent, second_portal_position);
                
            return;
        }

        //Barion
        if (player.name == "Barion")
        {
            Teleport(barion.gameObject, barion.agent, second_portal_position);
            return;
        }

        //Nyx
        if (player.name == "Nyx")
        {
            if(nyx.GetState() == nyx.dash_state)
            {
                //Update destination of the player                
                Vector3 position = nyx.transform.position;
                Vector3 destination = nyx.dash_state.destination;

                Vector3 distance = destination - position;
                position = new Vector3(second_portal_position.x, position.y, second_portal_position.z); //Teleport
                nyx.transform.position = position; //Position updated after crossing the portal
                nyx.dash_state.destination = position + distance; //New pathfinding
            }
            else
            {
                Teleport(nyx.gameObject, nyx.agent, second_portal_position);
            }
            return;
        }

    }

    /// <summary>
    /// Teleports a player to the next portal updating the navmesh agent
    /// </summary>
    private void Teleport(GameObject player, NavMeshAgent _agent, Vector3 portal_position)
    {
        //Update destination of the player                
        Vector3 position = player.transform.position;
        NavMeshAgent agent = _agent;

        Vector3 distance = agent.destination - position;
        position = new Vector3(portal_position.x, position.y,portal_position.z); //Teleport
        player.transform.position = position; //Position updated after crossing the portal
        agent.Warp(position);

        RaycastHit hit;
        if (Physics.Raycast(position, agent.destination.normalized, out hit, distance.magnitude, environment_collision_layer) == false)
        {
            agent.SetDestination(position + distance); //New pathfinding
        }
        else
        {
            agent.SetDestination(hit.point); //Something between the desired destination;
        }
    }

    //Characters selection
    public bool IsBarionSelected()
    {
        return barion.is_selected;
    }

    public bool IsCosmoSelected()
    {
        return cosmo.is_selected;
    }

    public bool IsNyxSelected()
    {
        return nyx.is_selected;
    }

    /// <summary>
    /// Stops all characters to follow Barion if they were, because the shield is destroyed.
    /// </summary>
    public void ShieldDisabled()
    {
        if(nyx.GetState() == nyx.chained_state)
        {
            nyx.chained_state.ToIdleState();
        }

        if(cosmo.GetState() == cosmo.chained_state)
        {
            cosmo.chained_state.ToIdleState();
        }
    }
}
