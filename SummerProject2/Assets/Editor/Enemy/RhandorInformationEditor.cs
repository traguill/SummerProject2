using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorInformationEditor : Editor
{
    private RhandorController rhandor;
    private IRhandorStates state;
    private Vector3[] path_neutral_positions, path_alarm_positions;

    void OnEnable()
    {
        rhandor = target as RhandorController;
        
        Transform[] path = rhandor.neutral_path.transform.getChilds();
        path_neutral_positions = new Vector3[path.Length];

        for (int i = 0; i < path.Length; ++i)
        {
            path_neutral_positions[i] = path[i].transform.position;
        }

        path = rhandor.alert_path.transform.getChilds();
        path_alarm_positions = new Vector3[path.Length];
       
        for (int i = 0; i < path.Length; ++i)
        {
            path_alarm_positions[i] = path[i].transform.position;
        }       
    }

    // CRZ -> I don't like the layout!
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();      
    }

    [ExecuteInEditMode]
    void OnSceneGUI()
    {
        ShowState();
        ShowPatrols();
    }

    private void ShowState()
    {
        state = rhandor.GetState();
        Vector3 draw_pos = new Vector3(rhandor.transform.position.x, 0, rhandor.transform.position.z - 1);

        //Show the current state in the editor
        if (state == rhandor.idle_state)
            Handles.Label(draw_pos, "IDLE");
        else if (state == rhandor.patrol_state)
            Handles.Label(draw_pos, "PATROL");
        else if (state == rhandor.alert_state)
            Handles.Label(draw_pos, "ALERT");
        else if (state == rhandor.spotted_state)
            Handles.Label(draw_pos, "SPOTTED");
        else if (state == rhandor.corpse_state)
            Handles.Label(draw_pos, "CORPSE");        
    }

    private void ShowPatrols()
    {
        // ---------------- For neutral patrols ----------------
        Handles.color = Color.white;
        Vector3[] line_segments = new Vector3[path_neutral_positions.Length * 2];
        int point_index = 0;

        for (int i = 0; i < path_neutral_positions.Length - 1; i++)
        {
            line_segments[point_index++] = path_neutral_positions[i];
            line_segments[point_index++] = path_neutral_positions[i + 1];
        }

        // We close the patrol loop
        line_segments[point_index++] = path_neutral_positions[path_neutral_positions.Length - 1];
        line_segments[point_index] = path_neutral_positions[0];

        Handles.DrawDottedLines(line_segments, 1.5f);

        for (int i = 0; i < path_neutral_positions.Length; i++)
        {
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 1, 1, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(path_neutral_positions[i]);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint: " + (i + 1));
            Handles.EndGUI();
            // Cones to indicate positions
            GUI.color = Color.white;
            Handles.ConeCap(0, path_neutral_positions[i], Quaternion.Euler(-90, 0, 0), 1);
        }

        // ---------------- For alert patrols ----------------
        Handles.color = Color.red;
        line_segments = new Vector3[path_alarm_positions.Length * 2];
        point_index = 0;

        for (int i = 0; i < path_alarm_positions.Length - 1; i++)
        {
            line_segments[point_index++] = path_alarm_positions[i];
            line_segments[point_index++] = path_alarm_positions[i + 1];
        }

        // We close the patrol loop
        line_segments[point_index++] = path_alarm_positions[path_alarm_positions.Length - 1];
        line_segments[point_index] = path_alarm_positions[0];


        Handles.DrawDottedLines(line_segments, 1.5f);

        for (int i = 0; i < path_alarm_positions.Length; i++)
        {
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 0, 0, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(path_alarm_positions[i]);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint: " + (i + 1));
            Handles.EndGUI();
            // Cones to indicate positions
            GUI.color = Color.red;
            Handles.ConeCap(0, path_alarm_positions[i], Quaternion.Euler(-90, 0, 0), 1);
        }
    }
}
