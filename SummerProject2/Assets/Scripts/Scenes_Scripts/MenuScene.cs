using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour {
	
	// Update is called once per frame
	void Update () 
    {
        //Shortcut to quit
	    if(Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    public void ButtonQuit()
    {
        Application.Quit();
    }

    public void ButtonLevel1()
    {
        SceneManager.LoadScene("Level 02_NEW"); //TODO: implement a level selector
    }

}
