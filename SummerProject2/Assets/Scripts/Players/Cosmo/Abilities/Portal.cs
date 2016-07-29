using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{

  
    public Transform second_portal = null; //The other portal

    public GameObject obj_crossing; //Object crossing the portal

    public PortalController controller;

	void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == Tags.player && coll.gameObject != obj_crossing)
        {
            if(second_portal != null)
            {
                controller.CrossingPortal(coll.gameObject, this); //Tell the controller to ignore the object for future collisions with the other portal
                

                //Update destination of the player
                Vector3 position = coll.transform.position;
                NavMeshAgent agent = coll.GetComponent<NavMeshAgent>();

                Vector3 distance = agent.destination - position;
                position = new Vector3(second_portal.position.x, position.y, second_portal.position.z); //Teleport
                coll.transform.position = position; //Position updated after crossing the portal
                agent.SetDestination(position + distance); //New pathfinding
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if(coll.tag == Tags.player)
        {
            obj_crossing = null; //The object has already cross the portal.
        }
    }


    public void Selected()
    {
        GetComponent<RadialMenu_ObjectInteractable>().OnInteractableClicked();
    }

    /// <summary>
    /// This method is called when a portal is selected and want to destroy it.
    /// </summary>
    public void DestroyPortal(GameObject obj)
    {
        controller.DestroyPortals(); //Calls the portal controller to destroy the portals
    }
}
