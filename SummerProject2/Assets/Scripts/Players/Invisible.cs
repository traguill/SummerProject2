using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the invisibility of a player.
/// </summary>
public class Invisible : MonoBehaviour
{
    bool is_invisible = false;
    public float invisible_time = 2.0f; //Time that the player remains invisible
    float timer = 0.0f; //Current time invisible
    
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
        Color new_color = GetComponent<SpriteRenderer>().color; //TODO: for now only change the alpha color of the sprite. Make an awesome animation for this and replace it.
        new_color.a = 1.0f;
        GetComponent<SpriteRenderer>().color = new_color;
    }

    /// <summary>
    /// Turns a character invisible and do all the animations.
    /// </summary>
    void TurnInvisibleAnim()
    {
        Color new_color = GetComponent<SpriteRenderer>().color; //TODO: for now only change the alpha color of the sprite. Make an awesome animation for this and replace it.
        new_color.a = 0.4f;
        GetComponent<SpriteRenderer>().color = new_color;
    }
}
