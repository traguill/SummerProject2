using UnityEngine;
using System.Collections;

public class BulletTime : MonoBehaviour {

    public float amount_slow = 0.5f;

    bool enabled = false;
	
	// Update is called once per frame
	void Update () 
    {
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            enabled = !enabled;

            if (enabled == true)
                Time.timeScale = amount_slow;
            else
                Time.timeScale = 1.0f; //Normal time scale
        }
	
	}
}
