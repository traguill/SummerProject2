using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorInformationEditor : Editor
{
    private RhandorController rhandor;
    private IRhandorStates state;

    private bool neutral_expanded = false, alert_expanded = false;
    private GameObject old_neutral_path, old_alert_path;    

    void OnEnable()
    {
        rhandor = target as RhandorController;
   
        rhandor.LoadNeutralPatrol();
        rhandor.LoadAlertPatrol();        
    }

    override public void OnInspectorGUI()
    {
        ShowInspectorPatrolControls();
    }

    [ExecuteInEditMode]
    void OnSceneGUI()
    {
        ShowState();
        ShowPatrols();
        ShowSupportCallRadius();     
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
        else if (state == rhandor.support_state)
            Handles.Label(draw_pos, "SUPPORT");
        else if (state == rhandor.corpse_state)
            Handles.Label(draw_pos, "CORPSE");  
        else
            Handles.Label(draw_pos, "UNKNOWN");
    }

    private void ShowPatrols()
    {
        // ---------------- For neutral patrols ----------------
        if(rhandor.neutral_patrol.static_patrol)
        {
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 1, 1, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(rhandor.initial_position);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint");
            Handles.EndGUI();
            // Cones to indicate positions
            Handles.color = Color.white;
            Handles.ConeCap(0, rhandor.initial_position, Quaternion.Euler(-90, 0, 0), 1);
        }
        else
        {
            Vector3[] line_segments = new Vector3[rhandor.neutral_patrol.Length * 2];
            int point_index = 0;

            for (int i = 0; i < rhandor.neutral_patrol.Length - 1; i++)
            {
                line_segments[point_index++] = rhandor.neutral_patrol.path[i];
                line_segments[point_index++] = rhandor.neutral_patrol.path[i + 1];
            }

            // We close the patrol loop
            if (!rhandor.neutral_patrol.loop)
            {
                line_segments[point_index++] = rhandor.neutral_patrol.path[rhandor.neutral_patrol.size - 1];
                line_segments[point_index] = rhandor.neutral_patrol.path[0];
            }

            Handles.color = Color.white;
            Handles.DrawDottedLines(line_segments, 1.5f);

            for (int i = 0; i < rhandor.neutral_patrol.Length; i++)
            {
                // Label indicating the Waypoint number
                Handles.BeginGUI();
                GUI.color = new Color(1, 1, 1, 0.75f);
                Vector2 gui_point = HandleUtility.WorldToGUIPoint(rhandor.neutral_patrol.path[i]);
                Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
                GUI.Box(rect, "Waypoint: " + (i + 1));
                Handles.EndGUI();
                // Cones to indicate positions
                Handles.color = Color.white;
                Handles.ConeCap(0, rhandor.neutral_patrol.path[i], Quaternion.Euler(-90, 0, 0), 1);
            }
        }

        // ---------------- For alert patrols ----------------
        if (rhandor.alert_patrol.static_patrol)
        {    
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 0, 0, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(rhandor.initial_position);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint");            
            Handles.EndGUI();

            // Cones to indicate positions
            Handles.color = Color.red;
            Handles.ConeCap(0, rhandor.initial_position, Quaternion.Euler(-90, 0, 0), 1);
        }
        else
        {
            Vector3[] line_segments = new Vector3[rhandor.alert_patrol.Length * 2];
            int point_index = 0;

            for (int i = 0; i < rhandor.alert_patrol.Length - 1; i++)
            {
                line_segments[point_index++] = rhandor.alert_patrol.path[i];
                line_segments[point_index++] = rhandor.alert_patrol.path[i + 1];
            }

            // We close the alert patrol loop
            if (!rhandor.alert_patrol.loop)
            {
                line_segments[point_index++] = rhandor.alert_patrol.path[rhandor.alert_patrol.Length - 1];
                line_segments[point_index] = rhandor.alert_patrol.path[0];
            }

            Handles.color = Color.red;
            Handles.DrawDottedLines(line_segments, 1.5f);

            for (int i = 0; i < rhandor.alert_patrol.Length; i++)
            {
                // Label indicating the Waypoint number
                Handles.BeginGUI();
                GUI.color = new Color(1, 0, 0, 0.75f);
                Vector2 gui_point = HandleUtility.WorldToGUIPoint(rhandor.alert_patrol.path[i]);
                Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
                GUI.Box(rect, "Waypoint: " + (i + 1));
                Handles.EndGUI();
                // Cones to indicate positions
                Handles.color = Color.red;
                Handles.ConeCap(1, rhandor.alert_patrol.path[i], Quaternion.Euler(-90, 0, 0), 1);
            }
        }            
    }

    private void ShowSupportCallRadius()
    {
        Handles.color = Color.blue;
        Handles.DrawWireArc(rhandor.transform.position, Vector3.up, Vector3.forward, 360, rhandor.ask_for_help_radius);
    }

    private void ShowInspectorPatrolControls()
    { 
        Undo.RecordObject(rhandor, "NeutralPatrolInspector");
        Undo.RecordObject(rhandor, "AlertPatrolInspector");
        Undo.RecordObject(rhandor, "SupportPatrolInspector");

        NeutralPatrolInspector();
        AlertPatrolInspector();
        SupportPatrolInspector();
    }
    
    private void NeutralPatrolInspector()
    {
        // ---- Neutral patrol editor information ---
        neutral_expanded = EditorGUILayout.Foldout(neutral_expanded, "Neutral patrol");
        if (neutral_expanded)
        {
            // Patrols attached as GameObjects and number of waypoints
            rhandor.neutral_patrol.static_patrol = EditorGUILayout.Toggle("Static", rhandor.neutral_patrol.static_patrol);
            if (rhandor.neutral_patrol.static_patrol)
            {
                EditorGUILayout.LabelField("Waypoints: 1");
                rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                rhandor.neutral_path = EditorGUILayout.ObjectField("Path", rhandor.neutral_path, typeof(GameObject), true) as GameObject;
                if (old_neutral_path != rhandor.neutral_path)
                {
                    rhandor.LoadNeutralPatrol();
                    old_neutral_path = rhandor.neutral_path;
                }

                if (rhandor.neutral_patrol.loop)
                    EditorGUILayout.LabelField("Waypoints: " + ((2 * rhandor.neutral_patrol.Length) - 2));
                else
                    EditorGUILayout.LabelField("Waypoints: " + rhandor.neutral_patrol.Length);
                EditorGUILayout.EndHorizontal();

                // Speeds information and patrol loop option
                EditorGUILayout.BeginHorizontal();
                rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
                if (rhandor.neutral_patrol.Length > 2)
                    rhandor.neutral_patrol.loop = EditorGUILayout.Toggle("Loop", rhandor.neutral_patrol.loop);
                else
                    rhandor.neutral_patrol.loop = false;
                EditorGUILayout.EndHorizontal();

                // Synchronous activity
                if (rhandor.is_synchronized_neutral = EditorGUILayout.Toggle("Patrol synchronized", rhandor.is_synchronized_neutral))
                {
                    rhandor.synchronized_neutral_Rhandor = EditorGUILayout.ObjectField("Synchronized Rhandor", rhandor.synchronized_neutral_Rhandor, typeof(GameObject), true) as GameObject;
                    if (rhandor.synchronized_neutral_Rhandor != null)
                    {
                        RhandorController check = rhandor.synchronized_neutral_Rhandor.GetComponent<RhandorController>();
                        if (check == null || check == rhandor)
                        {
                            Debug.Log("Synchronized patrol for " + rhandor.name + " is invalid!");
                            rhandor.synchronized_neutral_Rhandor = null;
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            if (rhandor.neutral_patrol.static_patrol)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint 1:", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", rhandor.transform.position, GUILayout.Width(240));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < rhandor.neutral_patrol.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                    EditorGUILayout.Vector3Field("", rhandor.neutral_patrol.path[i], GUILayout.Width(240));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                    rhandor.neutral_patrol.stop_times[i] = EditorGUILayout.FloatField(rhandor.neutral_patrol.stop_times[i], GUILayout.Width(75));
                    if (GUILayout.Button("Reset", GUILayout.Width(55)))
                        rhandor.neutral_patrol.stop_times[i] = 0.0f;

                    EditorGUILayout.EndHorizontal();
                }

                if (rhandor.neutral_patrol.loop)
                {
                    for (int i = rhandor.neutral_patrol.Length - 2; i > 0; --i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                        EditorGUILayout.Vector3Field("", rhandor.neutral_patrol.path[i], GUILayout.Width(240));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                        rhandor.neutral_patrol.stop_times[i] = EditorGUILayout.FloatField(rhandor.neutral_patrol.stop_times[i], GUILayout.Width(75));
                        if (GUILayout.Button("Reset", GUILayout.Width(55)))
                            rhandor.neutral_patrol.stop_times[i] = 0.0f;

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
    }

    private void AlertPatrolInspector()
    {
        // ---- Alert patrol editor information ---
        alert_expanded = EditorGUILayout.Foldout(alert_expanded, "Alert patrol");
        if (alert_expanded)
        {
            rhandor.alert_patrol.static_patrol = EditorGUILayout.Toggle("Static", rhandor.alert_patrol.static_patrol);
            if (rhandor.alert_patrol.static_patrol)
            {
                EditorGUILayout.LabelField("Waypoints: 1");
                EditorGUILayout.BeginHorizontal();
                rhandor.alert_speed = EditorGUILayout.FloatField("Alert speed", rhandor.alert_speed, GUILayout.Width(160));
                rhandor.spotted_speed = EditorGUILayout.FloatField("Spotted speed", rhandor.spotted_speed, GUILayout.Width(160));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                rhandor.alert_path = EditorGUILayout.ObjectField("Path", rhandor.alert_path, typeof(GameObject), true) as GameObject;
                if (rhandor.alert_path != old_alert_path)
                {
                    rhandor.LoadAlertPatrol();
                    old_alert_path = rhandor.alert_path;
                }

                if (rhandor.alert_patrol.loop)
                    EditorGUILayout.LabelField("Waypoints: " + ((2 * rhandor.alert_patrol.Length) - 2));
                else
                    EditorGUILayout.LabelField("Waypoints: " + rhandor.alert_patrol.Length);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                rhandor.alert_speed = EditorGUILayout.FloatField("Alert speed", rhandor.alert_speed, GUILayout.Width(160));
                rhandor.spotted_speed = EditorGUILayout.FloatField("Spotted speed", rhandor.spotted_speed, GUILayout.Width(160));
                EditorGUILayout.EndHorizontal();
                if (rhandor.alert_patrol.Length > 2)
                    rhandor.alert_patrol.loop = EditorGUILayout.Toggle("Loop", rhandor.alert_patrol.loop);
                else
                    rhandor.alert_patrol.loop = false;
            }

            EditorGUILayout.Space();

            if (rhandor.alert_patrol.static_patrol)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint 1:", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", rhandor.transform.position, GUILayout.Width(240));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < rhandor.alert_patrol.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                    EditorGUILayout.Vector3Field("", rhandor.alert_patrol.path[i], GUILayout.Width(240));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                    rhandor.alert_patrol.stop_times[i] = EditorGUILayout.FloatField(rhandor.alert_patrol.stop_times[i], GUILayout.Width(75));
                    if (GUILayout.Button("Reset", GUILayout.Width(55)))
                        rhandor.alert_patrol.stop_times[i] = 0.0f;
                    EditorGUILayout.EndHorizontal();
                }

                if (rhandor.alert_patrol.loop)
                {
                    for (int i = rhandor.alert_patrol.Length - 2; i > 0; --i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                        EditorGUILayout.Vector3Field("", rhandor.alert_patrol.path[i], GUILayout.Width(240));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                        rhandor.alert_patrol.stop_times[i] = EditorGUILayout.FloatField(rhandor.alert_patrol.stop_times[i], GUILayout.Width(75));
                        if (GUILayout.Button("Reset", GUILayout.Width(55)))
                            rhandor.alert_patrol.stop_times[i] = 0.0f;

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
    }

    private void SupportPatrolInspector()
    {
        // Support information
        EditorGUILayout.LabelField("--- Support parameters ---");
        rhandor.max_num_of_helpers = EditorGUILayout.IntField("Nº support enemies", rhandor.max_num_of_helpers, GUILayout.Width(175));
        if (rhandor.max_num_of_helpers < 0) rhandor.max_num_of_helpers = 0;
        rhandor.ask_for_help_radius = EditorGUILayout.FloatField("Radius call", rhandor.ask_for_help_radius);
        if (rhandor.ask_for_help_radius < 0.0f) rhandor.ask_for_help_radius = 0.0f;
    }
}

    
