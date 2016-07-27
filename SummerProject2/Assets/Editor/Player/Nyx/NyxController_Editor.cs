using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NyxController))]
public class NyxController_Editor : Editor {

	void OnSceneGUI()
    {
        NyxController nyx = (NyxController)target;

        //Show the current state in the editor
        if(nyx.GetState() == nyx.idle_state)
            Handles.Label(nyx.transform.position, "IDLE");

        if (nyx.GetState() == nyx.walking_state)
            Handles.Label(nyx.transform.position, "WALKING");

        if (nyx.GetState() == nyx.killing_state)
            Handles.Label(nyx.transform.position, "KILLING");

        if (nyx.GetState() == nyx.dash_state)
            Handles.Label(nyx.transform.position, "DASH");

        if (nyx.GetState() == nyx.hiding_state)
            Handles.Label(nyx.transform.position, "HIDING");
    }
}
