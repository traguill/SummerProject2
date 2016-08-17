using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(CosmoController))]
public class CosmoControllerEditor : Editor 
{
    public void OnSceneGUI()
    {
        CosmoController cosmo = (CosmoController)target;

        //Show the current state in the editor
        if(cosmo.GetState() == cosmo.idle_state)
        {
            Handles.Label(cosmo.transform.position, "IDLE");
        }

        if (cosmo.GetState() == cosmo.walking_state)
        {
            Handles.Label(cosmo.transform.position, "WALKING");
        }

        if (cosmo.GetState() == cosmo.hiding_state)
        {
            Handles.Label(cosmo.transform.position, "HIDING");
        }

        if (cosmo.GetState() == cosmo.sensorial_state)
        {
            Handles.Label(cosmo.transform.position, "SENSORIAL");
        }

        if (cosmo.GetState() == cosmo.portal_state)
        {
            Handles.Label(cosmo.transform.position, "PORTAL");
        }


        Handles.color = Color.blue;

        Handles.DrawWireArc(cosmo.transform.position, Vector3.up, Vector3.forward, 360, cosmo.max_detection_radius);
    }
	
}
