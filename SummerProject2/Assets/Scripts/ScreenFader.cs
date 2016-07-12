using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {

    private RawImage screen_fader;
    public float seconds_for_fading;
    private bool is_fading;


    void Awake()
    {
        screen_fader = GetComponent<RawImage>();
        seconds_for_fading = 1.0f;
        is_fading = false;
    }

    void fadeToClear()
    {
        screen_fader.CrossFadeAlpha(0.0f, seconds_for_fading, false);
    }

    void fadeToBlack()
    {
        screen_fader.CrossFadeAlpha(1.0f, seconds_for_fading, false);        
    }
}
