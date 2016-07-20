using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour {

    private RawImage fade_img;
    private float fade_speed;
    private bool scene_starting;

    void Awake()
    {
        fade_img = GetComponent<RawImage>();
        fade_img.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fade_img.enabled = true;

        fade_speed = 1.5f;
        scene_starting = true;
    }

    void Update()
    {
        // If the scene is starting...
        if (scene_starting)
            // ... call the StartScene function.
            startScene();
    }

    void fadeToClear()
    {
        // Lerp the colour of the image between itself and transparent.
        fade_img.color = Color.Lerp(fade_img.color, Color.clear, fade_speed * Time.deltaTime);
    }


    void fadeToBlack()
    {
        // Lerp the colour of the image between itself and black.
        fade_img.color = Color.Lerp(fade_img.color, Color.black, fade_speed * Time.deltaTime);
    }


    void startScene()
    {
        // Fade the texture to clear.
        fadeToClear();

        // If the texture is almost clear...
        if (fade_img.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            fade_img.color = Color.clear;
            fade_img.enabled = false;

            // The scene is no longer starting.
            scene_starting = false;
        }
    }


    public IEnumerator EndSceneRoutine(int SceneNumber)
    {
        // Make sure the RawImage is enabled.
        fade_img.enabled = true;
        do
        {
            // Start fading towards black.
            fadeToBlack();

            // If the screen is almost black...
            if (fade_img.color.a >= 0.95f)
            {
                // ... reload the level
                SceneManager.LoadScene(SceneNumber);
                yield break;
            }
            else
            {
                yield return null;
            }
        } while (true);
    }

    public void EndScene(int SceneNumber)
    {
        scene_starting = false;
        StartCoroutine("EndSceneRoutine", SceneNumber);
    }
}   

