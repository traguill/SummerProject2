using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

//Script attached to an interactable object that will display a radial menu.

public class RadialMenu_ObjectInteractable : MonoBehaviour {

    //Radial Menu parameters of each button.
    [Serializable]
    public class Action
    {
        public Color color;
        public Sprite sprite;
        public string title;
        public OnEvent function; //Function to call when the button is pressed.
    }
    public string id; //Id to allow multiple menus on same object
    public Action[] options;

    //When an interactable object is clicked (mouse button down) this method is called.
    public void OnInteractableClicked(string[] display_options = null)
    {
        RadialMenu_Spawner.instance.SpawnMenu(this, display_options);
    }


}
