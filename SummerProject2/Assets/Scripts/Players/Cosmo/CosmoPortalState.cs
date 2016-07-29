using UnityEngine;
using System.Collections;
using System;

public class CosmoPortalState : ICosmoState
{
    private readonly CosmoController cosmo;

    PortalController portal_controller = null; //Handles both portals

    Color build_zone_col = new Color(0.0f, 1.0f, 0.0f, 0.5f); //Color of buildable zone
    Color no_build_col = new Color(1.0f, 0.0f, 0.0f, 0.5f); //Color of no buildable zone

    GameObject build_portal; //Portal reference to build
    Material build_portal_mat; 

    public CosmoPortalState(CosmoController cosmo_controller)
    {
        cosmo = cosmo_controller;
    }

    public void StartState()
    {

        //First time
        if (portal_controller == null)
        {
            portal_controller = GameObject.Instantiate(cosmo.portal_controller_prefab) as PortalController; //Create the controller
            portal_controller.cosmo_portal_controller = this;
        }
        else
        { //Both portals are already created?
            if (portal_controller.ConnectionFinished())
            {
                ToIdleState();
                return;
            }    
        }

        //Create build guide
        build_portal = GameObject.Instantiate(cosmo.build_portal_prefab) as GameObject;
        build_portal_mat = build_portal.GetComponent<MeshRenderer>().material;

    }

    public void UpdateState()
    {
        //Cosmo must be selected
        if(cosmo.is_selected == false)
        {
            CancelPortalCreation();
            return;
        }

        if(Input.GetAxis("Ability2") != 0)
        { //Ability 2 pressed

            //Update walkable zone on mouse position
            if(AbleToBuild())
            {
                build_portal_mat.color = build_zone_col;
                //Check build order
                if(Input.GetMouseButtonUp(1))
                {
                    bool finish = portal_controller.BuildPortal(build_portal.transform.position);

                    if(finish)
                    {
                        CancelPortalCreation();
                    }
                }
            }
            else
            {
                build_portal_mat.color = no_build_col;
            }
            

        }
        else
        { //Released
            CancelPortalCreation();
        }
    }

    public void ToHideState()
    {
        Debug.Log("Cosmo can't transition from PORTAL to HIDE");
    }

    public void ToIdleState()
    {
        cosmo.ChangeStateTo(cosmo.idle_state);
    }

    public void ToPortalState()
    {
        Debug.Log("Cosmo can't transition from PORTAL to PORTAL");
    }

    public void ToSensorialState()
    {
        Debug.Log("Cosmo can't transition from PORTAL to SENSORIAL");
    }

    public void ToWalkingState()
    {
        Debug.Log("Cosmo can't transition from PORTAL to WALKING");
    }


    /// <summary>
    /// Check under the mouse if the zone is able to build a portal
    /// </summary>
    private bool AbleToBuild()
    {
        bool ret = false;

        //1- Ray from mouse to floor. If something is between discard build.
        Ray ray_mouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_mouse;
        if (Physics.Raycast(ray_mouse, out hit_mouse, 100,  cosmo.portal_build_layers))
        {
           
            Vector3 point = new Vector3(hit_mouse.point.x, 1.5f, hit_mouse.point.z);
            build_portal.transform.position = point;

            if (cosmo.floor_layer == (cosmo.floor_layer | (1 << hit_mouse.transform.gameObject.layer))) //Nothing is between, floor hit
            {
                //2- Ray from player to mouse 3d point. Is visible for the player?
                Vector3 direction = point - cosmo.transform.position;
                float distance = Vector3.Distance(cosmo.transform.position, point);

                if(Physics.Raycast(cosmo.transform.position, direction, distance, cosmo.portal_build_layers) == false) //Nothing is between
                {
                    
                    //3- Sphere collision if there is enough space to build the portal
                    Collider[] objects = Physics.OverlapSphere(point, 0.5f);

                    if(objects.Length == 0)
                    {
                        return true;
                    }

                }
            }
        }

        return ret;
    }

    /// <summary>
    /// Destroy both portals, if exist.
    /// </summary>
    public void DestroyPortals()
    {
        if (portal_controller != null)
        {
            GameObject.Destroy(portal_controller.gameObject);

            portal_controller = null;
        }
    }

    /// <summary>
    /// Finishes the portal state.
    /// </summary>
    private void CancelPortalCreation()
    {
        //Destroy portal guide
        GameObject.Destroy(build_portal);

        build_portal = null;
        build_portal_mat = null;

        ToIdleState();
    }

}
