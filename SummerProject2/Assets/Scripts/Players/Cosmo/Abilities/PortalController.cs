using UnityEngine;
using System.Collections;

public class PortalController : MonoBehaviour
{
    Portal portal_a = null;
    Portal portal_b = null;

    public Portal portal_prefab; //Prefab to clone portals

    [HideInInspector]public CosmoPortalState cosmo_portal_controller; //Instance to the portal state

    /// <summary>
    /// Tells the other portal to ignore collisions with the game object crossing the portal.
    /// </summary>
	public void CrossingPortal(GameObject obj, Portal start)
    {
        if(start == portal_a)
        {
            portal_b.obj_crossing = obj;
        }
        else
        {
            portal_a.obj_crossing = obj;
        }
    }

    /// <summary>
    /// Builds a portal in the given position and returns true if both portals has been created
    /// </summary>
    public bool BuildPortal(Vector3 position)
    {
        if (portal_a == null)
        {
            portal_a = Instantiate(portal_prefab, position, new Quaternion()) as Portal;
            portal_a.transform.SetParent(transform);
            Portal portal_script = portal_a.GetComponent<Portal>();
            portal_script.controller = this;

            return false;
        }
        else
        {
            portal_b = Instantiate(portal_prefab, position, new Quaternion()) as Portal;
            portal_b.transform.SetParent(transform);

            Portal portal_script = portal_b.GetComponent<Portal>();
            portal_script.controller = this;
            portal_script.second_portal = portal_a.transform;
            portal_a.GetComponent<Portal>().second_portal = portal_b.transform;

            return true;
        }
    }
    /// <summary>
    /// Returns true if both portals are created.
    /// </summary>
    public bool ConnectionFinished()
    {
        if (portal_a != null && portal_b != null)
            return true;

        return false;
    }

    /// <summary>
    /// This method is called by a portal that has to be destroyed.
    /// </summary>
    public void DestroyPortals()
    {
        cosmo_portal_controller.DestroyPortals(); //Calls the state to actually destroy the portals.
    }
}
