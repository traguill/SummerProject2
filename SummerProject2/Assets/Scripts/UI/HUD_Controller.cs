using UnityEngine;
using System.Collections;

public class HUD_Controller : MonoBehaviour 
{
    public GameObject canvas; //Canvas instance

    //Effects --------------------------------------------------------------------------------------
    public Vector3 barion_effect_position; ///Start position of the effects of Barion
    public Vector3 cosmo_effect_position; ///Start position of the effects of Barion
    public Vector3 nyx_effect_position; ///Start position of the effects of Barion
                                        ///
    public float effect_position_distance; //Distance between effects of the same character
                                          

    //For now the maximum effects at the same time are 3
    static int max_effects = 3;

    bool []barion_effects = new bool[max_effects];
    bool []cosmo_effects = new bool[max_effects];
    bool []nyx_effects = new bool[max_effects];

    public GameObject effect_prefab; //HUD prefab for an effect

    //Images of the effects
    public Sprite invisible_image;
    public Sprite shield_image;
    //-----------------------------------------------------------------------------------------------

    void Start()
    {
        //Clear the effects
        for(int i = 0; i < max_effects; i++)
        {
            barion_effects[i] = false;
            cosmo_effects[i] = false;
            nyx_effects[i] = false;
        }
    }



    public int CreateEffect(Enums.CharactersEffects effect, Enums.Characters character, float duration)
    {
        Vector3 position = new Vector3();
        Vector3 delay_position;

        int id = 0;
        //Search best position by character
        switch(character)
        {
            case Enums.Characters.BARION:
                    
                delay_position = SearchBestPositionEffect(ref barion_effects, ref id);
                position = barion_effect_position + delay_position;
                break;

            case Enums.Characters.COSMO:

                delay_position = SearchBestPositionEffect(ref cosmo_effects, ref id);
                position = cosmo_effect_position + delay_position;
                break;

            case Enums.Characters.NYX:

                delay_position = SearchBestPositionEffect(ref nyx_effects, ref id);
                position = nyx_effect_position + delay_position;
                break;
        }

        //Create effect
        GameObject effect_created = Instantiate(effect_prefab) as GameObject;
        effect_created.transform.SetParent(canvas.transform);
        HUD_Effect effect_controller = effect_created.GetComponent<HUD_Effect>();

        Sprite image = null;

        switch(effect)
        {
            case Enums.CharactersEffects.INVISIBLE:
                image = invisible_image;
                break;
            case Enums.CharactersEffects.SHIELD:
                image = shield_image;
                break;
        }

        effect_controller.Initialize(image, position, duration, this, character, id);

        return id;
        
    }

    public void EffectFinished(Enums.Characters character, int id)
    {
        switch (character)
        {
            case Enums.Characters.BARION:

                barion_effects[id] = false;
                break;

            case Enums.Characters.COSMO:

                cosmo_effects[id] = false;
                break;

            case Enums.Characters.NYX:

                nyx_effects[id] = false;
                break;
        }
    }


    /// <summary>
    /// Search for the unocupied position for an effect and returns the delay from the start position to create the effect
    /// </summary>
    private Vector3 SearchBestPositionEffect(ref bool []positions, ref int id)
    {
        Vector3 ret = new Vector3();
        for (int i = 0; i < max_effects; i++ )
        {
            if(positions[i] == false)
            {
                id = i;
                positions[i] = true;
                ret = new Vector3(i* effect_position_distance, 0, 0);
                break;
            }
        }

        return ret;
                   
    }


}
