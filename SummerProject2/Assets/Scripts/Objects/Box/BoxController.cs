using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoxController : MonoBehaviour
{
    BarionController barion; //Instance to Barion

    Dictionary<string, RadialMenu_ObjectInteractable> menus = new Dictionary<string, RadialMenu_ObjectInteractable>(); //List of menus that box will display with different interactions.

    string ground_id = "Ground"; //Id of the menu displayed when the box is in the ground.
    string carry_id = "Carry"; //Id of the menu displayed when the box is carried by Barion.

    void Awake()
    {
        barion = GameObject.Find("Barion").GetComponent<BarionController>();

        //Insert all menus in the dictionary
        RadialMenu_ObjectInteractable[] menus_scripts = GetComponents<RadialMenu_ObjectInteractable>();
        foreach(RadialMenu_ObjectInteractable menu_script in menus_scripts)
        {
            menus.Add(menu_script.id, menu_script);
        }

    }
	

    /// <summary>
    /// This method is called when the box is selected for first time.
    /// </summary>
    public void Selected()
    {
        if(barion.is_selected)
        {
            if(barion.current_state != barion.moving_box_state || barion.moving_box_state.IsCarryingBox() == false) //Barion doesn't have the box yet
            {
                menus[ground_id].OnInteractableClicked(); //Show Barion options to interact with box in the ground
            }
            else
            {
                menus[carry_id].OnInteractableClicked(); //Show Barion options to drop the box.
            }
           
        }
        
    }
}
