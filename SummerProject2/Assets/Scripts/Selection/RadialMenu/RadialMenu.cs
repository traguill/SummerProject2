using UnityEngine;
using System.Collections;

//Handles all the radial menu logic and creation.

public class RadialMenu : MonoBehaviour {

    public RadialButton button_prefab; //Prefab of button of menu.
    public RadialButton selected; //Current button selected.
    public float button_local_distance = 40; //Distance between buttons.

	public void SpawnButtons (RadialMenu_ObjectInteractable obj)
    {
        //Creates all the buttons in the menu with the interactable object information
        for(int i = 0; i < obj.options.Length; i++)
        {
            RadialButton new_button = Instantiate(button_prefab) as RadialButton;
            new_button.transform.SetParent(transform, false);

            float theta = (2 * Mathf.PI / obj.options.Length) * i;
            float x_pos = Mathf.Sin(theta);
            float y_pos = Mathf.Cos(theta);
            new_button.transform.localPosition = new Vector3(x_pos, y_pos, 0) * button_local_distance;

            new_button.background.color = obj.options[i].color;
            new_button.icon.sprite = obj.options[i].sprite;
            new_button.title = obj.options[i].title;
            new_button.main_menu = this;
            new_button.function = obj.options[i].function;
            new_button.interactable_object = obj.gameObject;
        }
  
	}
	

    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(selected)
            {
                selected.function.Invoke(selected.interactable_object); //Something is selected, call function attached
            }
            Destroy(gameObject); //Destroy the menu on mouse up
        }
    }
}
