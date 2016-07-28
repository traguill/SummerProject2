using UnityEngine;
using System;
using System.Collections;

public class Cooldown : MonoBehaviour
{
    //timer
    //seconds cooldown

    //startcounting
    //isReady
    [Serializable]
    public class AbilityCooldown
    {
        [HideInInspector]public float timer = 0.0f; //current time of the cooldown
        public float cooldown_seconds; //Cooldown of the ability
        [HideInInspector] public bool in_use = false; //The cooldown is counting time
    }

    public AbilityCooldown[] abilities;

    void Update()
    {
        for(int i = 0; i < abilities.Length; i++) //Update the timer af all the abilities
        {
            if (abilities[i].in_use == true)
            {
                abilities[i].timer += Time.deltaTime;

                if(abilities[i].timer >= abilities[i].cooldown_seconds) //Cooldown finished
                {
                    abilities[i].in_use = false;
                    abilities[i].timer = 0.0f;
                }
            }
        }
    }

    /// <summary>
    /// Starts the cooldown of the given ability by number (ability1, ability2 or ability3)
    /// </summary>
    public void StartCooldown(int num_ability)
    {
        if(num_ability - 1 <= abilities.Length) //Security comprobation to see if the given number is an ability
        {
            abilities[num_ability - 1].in_use = true;
        }
    }

    /// <summary>
    /// Returns if the ability is ready to use. If the number of the ability doesn't match with the available abilities return false.
    /// </summary>
    public bool AbilityIsReady(int num_ability)
    {
        if (num_ability - 1 <= abilities.Length) //Security comprobation to see if the given number is an ability
        {
            return !abilities[num_ability - 1].in_use; //If the cooldown is in use the ability is not ready, if is not in use the ability is ready
        }

        return false;
    }



	
}
