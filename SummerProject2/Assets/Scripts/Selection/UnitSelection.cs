using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*  Handles player selection and selectable objects selection.
 *  It's the main system of all the selection in the game
 */

public class UnitSelection : MonoBehaviour
{
    //Masks
    int floor_mask;
    int player_mask;
    public LayerMask selectable_object_mask;

    GameObject player_selected; //Current player selected

    GameObject object_selected; //Save if an object is selected to NOT update player selection

    //Players
    GameObject barion;
    GameObject nyx;
    GameObject cosmo;

    CameraMove cam;

    void Awake()
    {
        cam = Camera.main.GetComponent<CameraMove>();
        floor_mask = LayerMask.GetMask("Floor");
        player_mask = LayerMask.GetMask("Player");

        player_selected = null;

        barion = GameObject.Find("Barion");
        cosmo = GameObject.Find("Cosmo");
        nyx = GameObject.Find("Nyx");
    }
	
	// Update is called once per frame
	void Update ()
    {
        SelectObjects();

        if(Input.GetKeyUp(KeyCode.Space) && player_selected != null)
        {
            cam.MoveCameraTo(player_selected.transform.position);
        }

        if(object_selected == null) //No object is currently selected
        {
            //Shortcuts selection (high priority over mouse selection) TODO:change keycode for editable input
            if(Input.GetKeyUp(KeyCode.Alpha1)) //Barion
            {
                player_selected = barion;
                return;
            }

            if (Input.GetKeyUp(KeyCode.Alpha2)) //Cosmo
            {
                player_selected = cosmo;
                return;
            }

            if (Input.GetKeyUp(KeyCode.Alpha3)) //Nyx
            {
                player_selected = nyx;
                return;
            }

            UpdatePlayerSelection();
        }
           
        
	}


   
    
    /// <summary>
    /// Returns true if a Player is currently selected.
    /// </summary>
    public bool IsPlayerSelected(GameObject game_object)
    {
        if (player_selected == null) //Any player is selected
            return false;


        return (game_object.name == player_selected.name) ? true : false;
    }

  

    private void UpdatePlayerSelection()
    {
        //BUTTON_UP
        if (Input.GetMouseButtonUp(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, player_mask))
            {
                player_selected = hit.transform.gameObject;
            }
            else
            {
                player_selected = null;
            }
        }      
    }

    private void SelectObjects()
    {
        //Button down -> check selectableobject clicked
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, selectable_object_mask))
            {
                object_selected = hit.transform.gameObject;

                //Tell object that is being selected (all types of objects here)
                if (object_selected.tag == Tags.box)
                {
                    object_selected.GetComponent<BoxController>().Selected(); //Box
                    return;
                }

                if(object_selected.tag == Tags.corpse)
                {
                    object_selected.GetComponent<RadialMenu_ObjectInteractable>().OnInteractableClicked(); //Corpse
                    return;
                }
                object_selected.GetComponent<RadialMenu_ObjectInteractable>().OnInteractableClicked(); //Others
            }
            else
            {
                object_selected = null;
            }
        }

    }

    /// <summary>
    /// Removes a player of the current selection.
    /// </summary>
    public void DeselectPlayer(GameObject player)
    {
        if (player.name == player_selected.name)
            player_selected = null;
    }

    /// <summary>
    /// Removes the current selection and selects ONLY the given player.
    /// </summary>
    public void AutoSelectPlayer(GameObject player)
    {
        if(player != null && player.tag == Tags.player) //Make sure the object given is a viable
        {
            player_selected = player;
        }
    }
}
