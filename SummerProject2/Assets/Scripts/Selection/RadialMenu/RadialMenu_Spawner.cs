using UnityEngine;
using System.Collections;

//Creates a radial menu in the Canvas

public class RadialMenu_Spawner : MonoBehaviour {

    public static RadialMenu_Spawner instance;

    public RadialMenu menu_prefab; //Radial menu to create

	void Awake ()
    {
        instance = this; //Calls the canvas
	}
	
    /// <summary>
    /// Creates a radial menu with the parameters set in the interactable object
    /// </summary>
	public void SpawnMenu(RadialMenu_ObjectInteractable obj, string[] display_options = null)
    { //Instanciates the menu prefab

        RadialMenu menu = Instantiate(menu_prefab) as RadialMenu;
        menu.transform.SetParent(transform, false);

        menu.transform.position = Input.mousePosition;

        menu.SpawnButtons(obj, display_options);
    }
}
