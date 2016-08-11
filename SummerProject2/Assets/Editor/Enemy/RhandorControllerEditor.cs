using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorControllerEditor : Editor
{
    private IRhandorStates state;
    private RhandorController rhandor;
   
    void OnEnable()
    {
        rhandor = target as RhandorController;
    }

    void OnSceneGUI()
    {
        state = rhandor.GetState();

        //Show the current state in the editor
        if (state == rhandor.idle_state)
            Handles.Label(rhandor.transform.position, "IDLE");
        else if (state == rhandor.patrol_state)
            Handles.Label(rhandor.transform.position, "PATROL");
        else if (state == rhandor.alert_state)
            Handles.Label(rhandor.transform.position, "ALERT");
        else if (state == rhandor.spotted_state)
            Handles.Label(rhandor.transform.position, "SPOTTED");
        else if (state == rhandor.corpse_state)
            Handles.Label(rhandor.transform.position, "CORPSE");
    }
}

