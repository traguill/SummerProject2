using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuInGame : MonoBehaviour {

    public GameObject pause_menu;

    Image esc_img;

    void Awake()
    {
        esc_img = GetComponent<Image>();
    }

    void Start()
    {
        pause_menu.SetActive(false);
    }

    /// <summary>
    /// Escape button has been pressed. Open the in-game menu
    /// </summary>
    public void ButtonESC()
    {
        Time.timeScale = 0.0f; //Pause the time/game. TODO: check this.
        esc_img.enabled = false;
        pause_menu.SetActive(true);
    }

    /// <summary>
    /// Resume the game. U didn't expect this...
    /// </summary>
    public void ButtonResume()
    {
        Time.timeScale = 1.0f; 
        esc_img.enabled = true;
        pause_menu.SetActive(false);
    }

    /// <summary>
    /// Ends the game and returns to main menu
    /// </summary>
    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu"); //Save all data here before going to main menu
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
   public void ButtonQuit()
    {
        Application.Quit(); //Save all data before closing the game
    }
}
