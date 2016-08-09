using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(BarionController))]
public class BarionControllerEditor : Editor 
{
    void OnSceneGUI()
    {
        BarionController barion = (BarionController)target;

        //Show the current state in the editor
        if(barion.GetState() == barion.idle_state)
        {
            Handles.Label(barion.transform.position, "IDLE");
        }

        if (barion.GetState() == barion.walking_state)
        {
            Handles.Label(barion.transform.position, "WALKING");
        }

        if (barion.GetState() == barion.moving_box_state)
        {
            Handles.Label(barion.transform.position, "MOVING BOX");
        }

        if (barion.GetState() == barion.hiding_state)
        {
            Handles.Label(barion.transform.position, "HIDING");
        }

        if (barion.GetState() == barion.invisible_sphere_state)
        {
            Handles.Label(barion.transform.position, "INVISIBLE SPHERE");
        }

        if (barion.GetState() == barion.shield_state)
        {
            Handles.Label(barion.transform.position, "SHIELD");
        }
    }
	
}
