using UnityEngine;
using System.Collections;

public class BoxHideMenu_Spawner : MonoBehaviour {

    public static BoxHideMenu_Spawner instance;

    public BoxHideMenu menu_prefab;

	void Awake ()
    {
        instance = this;
	}
	
	public BoxHideMenu SpawnMenu(BoxHideInteractable obj, Vector3 spawn_position)
    {
        BoxHideMenu menu = Instantiate(menu_prefab) as BoxHideMenu;

        menu.transform.SetParent(transform, false);
        menu.transform.position = spawn_position;

        return menu;
    }
}
