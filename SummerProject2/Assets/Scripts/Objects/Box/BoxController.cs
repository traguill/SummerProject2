using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoxController : MonoBehaviour
{
    //Player instances
    BarionController barion;
    NyxController nyx;
    CosmoController cosmo;

    Dictionary<string, RadialMenu_ObjectInteractable> menus = new Dictionary<string, RadialMenu_ObjectInteractable>(); //List of menus that box will display with different interactions.

    string ground_id = "Ground"; //Id of the menu displayed when the box is in the ground.
    string carry_id = "Carry"; //Id of the menu displayed when the box is carried by Barion.

    UnitSelection selection_sys; //Instance to selection system

    string[] hide_options = { "Hide" }; //Buttons that will show in hide mode

    BoxHideInteractable hide_inst; //Hide controller instance

    void Awake()
    {
        barion = GameObject.Find("Barion").GetComponent<BarionController>();
        nyx = GameObject.Find("Nyx").GetComponent<NyxController>();
        cosmo = GameObject.Find("Cosmo").GetComponent<CosmoController>();

        selection_sys = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();

        //Insert all menus in the dictionary
        RadialMenu_ObjectInteractable[] menus_scripts = GetComponents<RadialMenu_ObjectInteractable>();
        foreach(RadialMenu_ObjectInteractable menu_script in menus_scripts)
        {
            menus.Add(menu_script.id, menu_script);
        }

        hide_inst = GetComponent<BoxHideInteractable>();

    }
	

    /// <summary>
    /// This method is called when the box is selected for first time.
    /// </summary>
    public void Selected()
    {
        //Barion options
        if(barion.is_selected && selection_sys.PlayersSelected() == 1)
        {
            if(barion.GetState() != barion.moving_box_state || barion.moving_box_state.IsCarryingBox() == false && barion.is_hide) //Barion doesn't have the box yet and its not hided
            {
                menus[ground_id].OnInteractableClicked(); //Show Barion options to interact with box in the ground
            }
            else
            {
                menus[carry_id].OnInteractableClicked(); //Show Barion options to drop the box.
            }
           
        }

        //Cosmo & Nyx options
        if((nyx.is_selected || cosmo.is_selected) && selection_sys.PlayersSelected() == 1)
        {
            menus[ground_id].OnInteractableClicked(hide_options);
        }
        
    }

    /// <summary>
    /// Handles the UI to show a character hided.
    /// </summary>
    public void HideCharacter(string name)
    {
        hide_inst.DisplayIcon(name);
    }

    /// <summary>
    /// Removes the UI icon of a character that is no longer hided in the box
    /// </summary>
    public void RemoveHideCharacter(string name)
    {
        hide_inst.HideIcon(name);
    }
}
