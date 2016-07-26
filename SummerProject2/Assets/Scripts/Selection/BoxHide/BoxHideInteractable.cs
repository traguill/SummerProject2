using UnityEngine;
using System;
using System.Collections;

public class BoxHideInteractable : MonoBehaviour {

    [Serializable]
    public class HideBut
    {
        public OnEventSimple function;
        public Sprite icon;
        public string name;
    }

    public HideBut[] options; //Array of buttons in the menu

    public float menu_position_delay; //Delay in the Y axis of the spawn of the menu.

    BoxHideMenu menu = null; //Menu created

    /// <summary>
    /// Displays the basic menu of the hiding objects in the box.
    /// </summary>
	public void DisplayHideMenu()
    {
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position); //Position of the menu in Canvas coordinates
        pos.y -= menu_position_delay;
        menu = BoxHideMenu_Spawner.instance.SpawnMenu(this, pos);
    }


    /// <summary>
    /// Shows the icon of the character inside the box.
    /// </summary>
    public void DisplayIcon(string icon)
    {
        //Search by name
        for(int i = 0; i < options.Length; i++)
        {
            if(options[i].name == icon)
            {
                if(menu == null)
                {
                    DisplayHideMenu(); //If the menu is not created yet, create it.
                }
                menu.DisplayIcon(this, i);
            }
        }
    }

    /// <summary>
    /// Hides the given icon of the character
    /// </summary>
    public void HideIcon(string icon)
    {
        //Search by name
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].name == icon)
            {
                if (menu != null)
                {
                    menu.HideIcon(this, i);
                }
            }
        }
    }

    /// <summary>
    /// Hides the menu
    /// </summary>
    public void HideMenu()
    {
        if(menu != null)
            menu.HideMenu();
    }


}
