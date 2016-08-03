using UnityEngine;
using System.Collections;

public class PlayersManager : MonoBehaviour
{

    //Players controller
    CosmoController cosmo;
    NyxController nyx;
    BarionController barion;

    void Awake()
    {
        cosmo = GameObject.Find("Cosmo").GetComponent<CosmoController>();
        nyx = GameObject.Find("Nyx").GetComponent<NyxController>();
        barion = GameObject.Find("Barion").GetComponent<BarionController>();
    }

	
    /// <summary>
    /// Sets the new destination for the player and handles all the conficts that could happen between states.
    /// </summary>
    public void CrossPortal(GameObject player, Vector3 second_portal_position)
    {
        //Cosmo
        if(player.name == "Cosmo")
        {
            //Update destination of the player                
            Vector3 position = cosmo.transform.position;
            NavMeshAgent agent = cosmo.agent;

            Vector3 distance = agent.destination - position;
            position = new Vector3(second_portal_position.x, position.y, second_portal_position.z); //Teleport
            cosmo.transform.position = position; //Position updated after crossing the portal
            agent.SetDestination(position + distance); //New pathfinding
            return;
        }

        //Barion
        if (player.name == "Barion")
        {
            //Update destination of the player                
            Vector3 position = barion.transform.position;
            NavMeshAgent agent = barion.agent;

            Vector3 distance = agent.destination - position;
            position = new Vector3(second_portal_position.x, position.y, second_portal_position.z); //Teleport
            barion.transform.position = position; //Position updated after crossing the portal
            agent.SetDestination(position + distance); //New pathfinding
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
                //Update destination of the player                
                Vector3 position = nyx.transform.position;
                NavMeshAgent agent = nyx.agent;

                Vector3 distance = agent.destination - position;
                position = new Vector3(second_portal_position.x, position.y, second_portal_position.z); //Teleport
                nyx.transform.position = position; //Position updated after crossing the portal
                agent.SetDestination(position + distance); //New pathfinding
            }
            return;
        }

    }
}
