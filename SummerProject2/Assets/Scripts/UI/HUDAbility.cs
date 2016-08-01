using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDAbility : MonoBehaviour
{

    public Cooldown controller; //Handles the cooldown of the ability. Here just ask for info.
    public int id; //Id of the ability
    public Image cooldown_clock;  //Clock of the cooldown.

    bool cooldown_active = false; //Is the ability in cooldown
	
	void Update ()
    {
        if(cooldown_active == false)
        {
            if (controller.abilities[id].in_use == true) //Cooldown started
            {
                cooldown_active = true;
                cooldown_clock.fillAmount = 1;
            }
                
        }
        else
        {
            //Update clock animation
            float clock_value = controller.abilities[id].timer / controller.abilities[id].cooldown_seconds;
            cooldown_clock.fillAmount = 1 - clock_value;

            if (controller.abilities[id].in_use == false) //Cooldown finished
            {
                cooldown_active = false;
                cooldown_clock.fillAmount = 0;
            }
        }
        
	}
}
