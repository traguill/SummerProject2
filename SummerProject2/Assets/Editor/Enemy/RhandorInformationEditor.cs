﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorInformationEditor : Editor
{
    private RhandorController rhandor;
    private SerializedObject rhandor_obj;
    private IRhandorStates state;
    private Vector3[] path_neutral_positions, path_alarm_positions;

    private bool neutral_expanded = false, alert_expanded = false;

    void OnEnable()
    {
        rhandor = target as RhandorController;

        // ---- Neutral patrol initialization for editor ----

        Transform[] path = rhandor.neutral_path.transform.getChilds();
        path_neutral_positions = new Vector3[path.Length];
        
        for (int i = 0; i < path.Length; ++i)
        {
            path_neutral_positions[i] = path[i].transform.position;
        }

        rhandor.num_neutral_waypoints = path_neutral_positions.Length;

        // ---- Neutral patrol initialization for editor ----

        path = rhandor.alert_path.transform.getChilds();
        path_alarm_positions = new Vector3[path.Length];

        for (int i = 0; i < path.Length; ++i)
        {
            path_alarm_positions[i] = path[i].transform.position;
        }

        rhandor.num_alert_waypoints = path_alarm_positions.Length;
    }

    // CRZ -> I don't like the layout!
    override public void OnInspectorGUI()
    {
        CheckArrayChanges();
       
        // ---- Neutral patrol editor information ---
        neutral_expanded = EditorGUILayout.Foldout(neutral_expanded, "Neutral patrol");
        if (neutral_expanded)
        {
            rhandor.neutral_path = EditorGUILayout.ObjectField("Neutral patrol path", rhandor.neutral_path, typeof(GameObject), true) as GameObject;

            EditorGUILayout.BeginHorizontal();
            rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
            EditorGUILayout.LabelField("Waypoints: " + path_neutral_positions.Length);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < path_neutral_positions.Length; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", path_neutral_positions[i]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                rhandor.stopping_time_neutral_patrol[i] = EditorGUILayout.FloatField(rhandor.stopping_time_neutral_patrol[i], GUILayout.Width(75));
                EditorGUILayout.EndHorizontal();
            }
        }

        // ---- Alert patrol editor information ---
        alert_expanded = EditorGUILayout.Foldout(alert_expanded, "Alert patrol");
        if (alert_expanded)
        {
            rhandor.alert_path = EditorGUILayout.ObjectField("Alert patrol path", rhandor.alert_path, typeof(GameObject), true) as GameObject;

            EditorGUILayout.BeginHorizontal();
            rhandor.alert_speed = EditorGUILayout.FloatField("Alert speed", rhandor.alert_speed, GUILayout.Width(160));
            rhandor.spotted_speed = EditorGUILayout.FloatField("Spotted speed", rhandor.spotted_speed, GUILayout.Width(160));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Waypoints: " + path_alarm_positions.Length);

            for (int i = 0; i < path_alarm_positions.Length; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", path_alarm_positions[i]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                rhandor.stopping_time_alert_patrol[i] = EditorGUILayout.FloatField(rhandor.stopping_time_alert_patrol[i], GUILayout.Width(75));
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorUtility.SetDirty(rhandor);
    }

    [ExecuteInEditMode]
    void OnSceneGUI()
    {
        ShowState();
        ShowPatrols();
    }

    /// <summary>
    /// Adapts the two arrays that control the stopping times of the enemies to allocate 
    /// the different points that the neutral and the alert paths contain.
    /// </summary>

    private void CheckArrayChanges()
    {
        rhandor.num_neutral_waypoints = path_neutral_positions.Length;
        if (rhandor.stopping_time_neutral_patrol.Length != rhandor.num_neutral_waypoints)
        {
            float[] new_array = new float[rhandor.num_neutral_waypoints];
            for (int x = 0; x < rhandor.num_neutral_waypoints; ++x)
            {
                if (rhandor.stopping_time_neutral_patrol.Length > x)
                {
                    new_array[x] = rhandor.stopping_time_neutral_patrol[x];
                }
            }
            rhandor.stopping_time_neutral_patrol = new_array;
        }

        rhandor.num_alert_waypoints = path_alarm_positions.Length;
        if (rhandor.stopping_time_alert_patrol.Length != rhandor.num_alert_waypoints)
        {
            float[] new_array = new float[rhandor.num_alert_waypoints];
            for (int x = 0; x < rhandor.num_alert_waypoints; ++x)
            {
                if (rhandor.stopping_time_alert_patrol.Length > x)
                {
                    new_array[x] = rhandor.stopping_time_alert_patrol[x];
                }
            }
            rhandor.stopping_time_alert_patrol = new_array;
        }
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
