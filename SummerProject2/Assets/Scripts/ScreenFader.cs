using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//public class SceneFadeInOut : MonoBehaviour
//{
//    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.


//    private bool sceneStarting = true;      // Whether or not the scene is still fading in.


//    void Awake()
//    {
//        // Set the texture so that it is the the size of the screen and covers it.
//        guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
//    }


//    void Update()
//    {
//        // If the scene is starting...
//        if (sceneStarting)
//            // ... call the StartScene function.
//            StartScene();
//    }


//    void FadeToClear()
//    {
//        // Lerp the colour of the texture between itself and transparent.
//        guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
//    }


//    void FadeToBlack()
//    {
//        // Lerp the colour of the texture between itself and black.
//        guiTexture.color = Color.Lerp(guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
//    }


//    void StartScene()
//    {
//        // Fade the texture to clear.
//        FadeToClear();

//        // If the texture is almost clear...
//        if (guiTexture.color.a <= 0.05f)
//        {
//            // ... set the colour to clear and disable the GUITexture.
//            guiTexture.color = Color.clear;
//            guiTexture.enabled = false;

//            // The scene is no longer starting.
//            sceneStarting = false;
//        }
//    }


//    public void EndScene()
//    {
//        // Make sure the texture is enabled.
//        guiTexture.enabled = true;

//        // Start fading towards black.
//        FadeToBlack();

//        // If the screen is almost black...
//        if (guiTexture.color.a >= 0.95f)
//            // ... reload the level.
//            Application.LoadLevel(0);
//    }
//}

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
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKey(KeyCode.G))
        {
            fadeToClear();
            Debug.Log("Press G!");
        }

        if(Input.GetKey(KeyCode.H))
        {
            fadeToBlack();
            Debug.Log("Press H!");
        }
             
	}
}
