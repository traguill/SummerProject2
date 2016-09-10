using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the invisibility of a player.
/// </summary>
public class Invisible : MonoBehaviour
{
    HUD_Controller hud;

    bool is_invisible = false;
    public float invisible_time = 2.0f; //Time that the player remains invisible
    float timer = 0.0f; //Current time invisible

    public Enums.Characters character; //Current character

    public GameObject invisible_sphere; //Invisible sphere prefab to create the effect

    void Awake()
    {
        hud = GameObject.Find(Objects.HUD).GetComponent<HUD_Controller>();


        invisible_sphere.SetActive(false);
    }

    void Update()
    {
        if(is_invisible)
        {
            timer += Time.deltaTime;
            if (timer >= invisible_time)
            {
                TurnVisibleAnim();
                is_invisible = false;
            }
                
        }
    }
    /// <summary>
    /// Turns a character invisible
    /// </summary>
    public void TurnInvisible()
    {
        if(is_invisible == false)
        {
            hud.CreateEffect(Enums.CharactersEffects.INVISIBLE, character, invisible_time);
            TurnInvisibleAnim();
            is_invisible = true;
            timer = 0.0f; //Reset timer
        }

    }


    /// <summary>
    /// Returns true if the character is currently invisible.
    /// </summary>
    public bool IsInvisible()
    {
        return is_invisible;
    }

    /// <summary>
    /// Turns a character visible and do all the animations.
    /// </summary>
    void TurnVisibleAnim()
    {
        invisible_sphere.SetActive(false);
    }

    /// <summary>
    /// Turns a character invisible and do all the animations.
    /// </summary>
    void TurnInvisibleAnim()
    {
        invisible_sphere.SetActive(true);
    }
}
