using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{

  
    public Transform second_portal = null; //The other portal

    public GameObject obj_crossing; //Object crossing the portal

    public PortalController controller;

	void OnTriggerEnter(Collider coll)
    {

        if(coll.gameObject != obj_crossing && second_portal != null)
        {
            if (coll.tag == Tags.player)
            {
                controller.CrossingPortal(coll.gameObject, this, true); //Tell the controller to ignore the object for future collisions with the other portal             
            }
            else
            {
                controller.CrossingPortal(coll.gameObject, this, false);
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
