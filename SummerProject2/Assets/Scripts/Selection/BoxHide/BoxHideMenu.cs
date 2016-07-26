using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoxHideMenu : MonoBehaviour
{
    public BoxHideButton button_prefab;
    public BoxHideButton selected;

    Vector3 pos_A = new Vector3(-7.2f, -1.17f); //Icon position left
    Vector3 pos_B = new Vector3(7.2f, -1.17f); //Icon position right

    int current_icons = 0; //Current number of displayed icons in the menu (max 2)

    List<BoxHideButton> buttons = new List<BoxHideButton>(); //List of buttons displayed at the moment
         
    /// <summary>
    /// Creates a button to display in the box hide menu from a given id.
    /// </summary>
	public void DisplayIcon(BoxHideInteractable obj, int id)
    {
        if (current_icons == 2) //Max number of icons
            return;

        BoxHideButton button = Instantiate(button_prefab) as BoxHideButton;
        button.transform.SetParent(transform, false);

        button.icon.sprite = obj.options[id].icon;
        button.function = obj.options[id].function;
        button.name = obj.options[id].name;

        //Seach free position
        if(current_icons == 0)
            button.transform.localPosition = pos_A;
        else
            button.transform.localPosition = pos_B;

        buttons.Add(button);
        current_icons++;
    }


    /// <summary>
    /// Removes a button in the box hide menu from a given id
    /// </summary>
    public void HideIcon(BoxHideInteractable obj, int id)
    {
        BoxHideButton button = buttons.Find(x => x.name == obj.options[id].name); //Find button to remove

        if(button != null) //Desired button to delete found
        {
            buttons.Remove(button); //Remove from list
            Destroy(button.gameObject); //Remove the object

            current_icons--;

            //Repositioning of the first icon to position A
            if(current_icons == 1)
            {
                buttons[0].transform.localPosition = pos_A;
            }

            if(current_icons == 0) //Remove the menu because no one is hide
            {
                HideMenu();
            }

        }
    }

    /// <summary>
    /// Removes the menu and all it's buttons.
    /// </summary>
    public void HideMenu()
    {
        Destroy(gameObject);
    }
}
