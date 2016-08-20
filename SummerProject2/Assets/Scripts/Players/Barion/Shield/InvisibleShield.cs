using UnityEngine;
using System.Collections;

public class InvisibleShield : MonoBehaviour
{
    [HideInInspector]public HUD_Controller hud;
    [HideInInspector] public int hud_id; //Id to the effect in the HUD
    public float life = 5; //Active time

    float timer = 0;

    [HideInInspector] public BarionController barion; //Barion reference

    bool key_up = false; //Saves previous button up before click the button again

    [HideInInspector] public Vector3 delay_pos = new Vector3(); //Delay of position respect barion
	
	// Update is called once per frame
	void Update ()
    {
        //Update life time
	    if(timer <= life)
        {
            timer += Time.deltaTime;
        }
        else
        {
            hud.EffectFinished(Enums.Characters.BARION, hud_id);
            Destroy(gameObject);
        }

        //Update position (follow barion)
        transform.position = barion.transform.position + delay_pos;


        //First creation, key up
        if (key_up == false && Input.GetAxis("Ability2") == 0)
            key_up = true;

        //Key pressed again to cancel ability
        if(barion.is_selected && Input.GetAxis("Ability2") != 0 && key_up == true)
        {
            Destroy(gameObject);
        }
	}
}
