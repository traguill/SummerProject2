using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDAbility : MonoBehaviour
{

    public Cooldown controller; //Handles the cooldown of the ability. Here just ask for info.
    public int id; //Id of the ability
    Image cooldown_clock;  //Clock of the cooldown.

    bool cooldown_active = false; //Is the ability in cooldown

    public HUD_Character character; //Character reference to handle selection

    public Sprite selected_sprite;
    public Sprite unselected_sprite;

    bool selected = false; //Is the character selected

    void Awake()
    {
        cooldown_clock = GetComponent<Image>();
    }
	
	void Update ()
    {
        //Update selection animation
        if(character.selected != selected) //With this avoids setting the sprite every frame
        {
            if (character.selected == true) //Selected
            {
                cooldown_clock.sprite = selected_sprite;
                selected = true;
            }
            else //Unselected
            {
                cooldown_clock.sprite = unselected_sprite;
                selected = false;
            }
        }
        

        //Update cooldown animation
        if(cooldown_active == false)
        {
            if (controller.abilities[id].in_use == true) //Cooldown started
            {
                cooldown_active = true;
                cooldown_clock.fillAmount = 0;
            }
                
        }
        else
        {
            //Update clock animation
            float clock_value = controller.abilities[id].timer / controller.abilities[id].cooldown_seconds;
            cooldown_clock.fillAmount = clock_value;

            if (controller.abilities[id].in_use == false) //Cooldown finished
            {
                cooldown_active = false;
                cooldown_clock.fillAmount = 1;
            }
        }
        
	}
}
