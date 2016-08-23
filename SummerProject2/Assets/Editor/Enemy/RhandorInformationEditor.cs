using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorInformationEditor : Editor
{
    private RhandorController rhandor;
    private IRhandorStates state;
    private Vector3[] path_neutral_positions, path_alert_positions;
    //private bool paths_attached = false;

    private bool neutral_expanded = false, alert_expanded = false;
    

    void OnEnable()
    {
        rhandor = target as RhandorController;

        // Patrols initialization

        if(!rhandor.static_neutral_path)
        {
            // ---- Neutral patrol initialization for editor ----
            Transform[] path = rhandor.neutral_path.transform.getChilds();
            path_neutral_positions = new Vector3[path.Length];

            for (int i = 0; i < path.Length; ++i)
            {
                path_neutral_positions[i] = path[i].transform.position;
            }

            rhandor.num_neutral_waypoints = path_neutral_positions.Length;
        }

        if (!rhandor.static_alert_path)
        {
            // ---- Alert patrol initialization for editor ----
            Transform[] path = rhandor.alert_path.transform.getChilds();
            path_alert_positions = new Vector3[path.Length];

            for (int i = 0; i < path.Length; ++i)
            {
                path_alert_positions[i] = path[i].transform.position;
            }

            rhandor.num_alert_waypoints = path_alert_positions.Length;
        }       
    }

    // CRZ -> I don't like the layout!
    override public void OnInspectorGUI()
    {
        CheckArrayChanges();
        ShowInspectorPatrols();
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
        if(!rhandor.static_neutral_path)
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
        }

        if (!rhandor.static_alert_path)
        {
            rhandor.num_alert_waypoints = path_alert_positions.Length;
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
        if(rhandor.static_neutral_path)
        {
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 1, 1, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(rhandor.initial_position);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint");
            Handles.EndGUI();
            // Cones to indicate positions
            GUI.color = Color.white;
            Handles.ConeCap(0, rhandor.initial_position, Quaternion.Euler(-90, 0, 0), 1);
        }
        else
        {
            Vector3[] line_segments = new Vector3[path_neutral_positions.Length * 2];
            int point_index = 0;

            for (int i = 0; i < path_neutral_positions.Length - 1; i++)
            {
                line_segments[point_index++] = path_neutral_positions[i];
                line_segments[point_index++] = path_neutral_positions[i + 1];
            }

            // We close the patrol loop
            if (!rhandor.neutral_path_loop)
            {
                line_segments[point_index++] = path_neutral_positions[path_neutral_positions.Length - 1];
                line_segments[point_index] = path_neutral_positions[0];
            }

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
        }


        // ---------------- For alert patrols ----------------
        if (rhandor.static_alert_path)
        {
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 0, 0, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(rhandor.initial_position);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint");
            Handles.EndGUI();
            // Cones to indicate positions
            GUI.color = Color.red;
            Handles.ConeCap(0, rhandor.initial_position, Quaternion.Euler(-90, 0, 0), 1);
        }
        else
        {
            Handles.color = Color.red;
            Vector3[] line_segments = new Vector3[path_alert_positions.Length * 2];
            int point_index = 0;

            for (int i = 0; i < path_alert_positions.Length - 1; i++)
            {
                line_segments[point_index++] = path_alert_positions[i];
                line_segments[point_index++] = path_alert_positions[i + 1];
            }

            // We close the patrol loop
            if (!rhandor.alert_path_loop)
            {
                line_segments[point_index++] = path_alert_positions[path_alert_positions.Length - 1];
                line_segments[point_index] = path_alert_positions[0];
            }

            Handles.DrawDottedLines(line_segments, 1.5f);

            for (int i = 0; i < path_alert_positions.Length; i++)
            {
                // Label indicating the Waypoint number
                Handles.BeginGUI();
                GUI.color = new Color(1, 0, 0, 0.75f);
                Vector2 gui_point = HandleUtility.WorldToGUIPoint(path_alert_positions[i]);
                Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
                GUI.Box(rect, "Waypoint: " + (i + 1));
                Handles.EndGUI();
                // Cones to indicate positions
                GUI.color = Color.red;
                Handles.ConeCap(0, path_alert_positions[i], Quaternion.Euler(-90, 0, 0), 1);
            }
        }            
    }

    private void ShowInspectorPatrols()
    {
        // ---- Neutral patrol editor information ---
        neutral_expanded = EditorGUILayout.Foldout(neutral_expanded, "Neutral patrol");
        if (neutral_expanded)
        {
            // Patrols attached as GameObjects and number of waypoints
            rhandor.static_neutral_path = EditorGUILayout.Toggle("Static", rhandor.static_neutral_path);
            if (rhandor.static_neutral_path)
            {
                EditorGUILayout.LabelField("Waypoints: 1");
                rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                rhandor.neutral_path = EditorGUILayout.ObjectField("Path", rhandor.neutral_path, typeof(GameObject), true) as GameObject;

                if (rhandor.neutral_path_loop)
                    EditorGUILayout.LabelField("Waypoints: " + ((2 * path_neutral_positions.Length) - 2));
                else
                    EditorGUILayout.LabelField("Waypoints: " + path_neutral_positions.Length);
                EditorGUILayout.EndHorizontal();

                // Speeds information and patrol loop option
                EditorGUILayout.BeginHorizontal();
                rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
                if (path_neutral_positions.Length > 1)
                    rhandor.neutral_path_loop = EditorGUILayout.Toggle("Loop", rhandor.neutral_path_loop);
                else
                    rhandor.neutral_path_loop = false;
                EditorGUILayout.EndHorizontal();
            }             

            EditorGUILayout.Space();

            if (rhandor.static_neutral_path)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint 1:", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", rhandor.transform.position, GUILayout.Width(240));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < path_neutral_positions.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                    EditorGUILayout.Vector3Field("", path_neutral_positions[i], GUILayout.Width(240));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                    rhandor.stopping_time_neutral_patrol[i] = EditorGUILayout.FloatField(rhandor.stopping_time_neutral_patrol[i], GUILayout.Width(75));
                    if (GUILayout.Button("Reset", GUILayout.Width(55)))
                        rhandor.stopping_time_neutral_patrol[i] = 0.0f;

                    EditorGUILayout.EndHorizontal();
                }

                if (rhandor.neutral_path_loop)
                {
                    for (int i = path_neutral_positions.Length - 2; i > 0; --i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                        EditorGUILayout.Vector3Field("", path_neutral_positions[i], GUILayout.Width(240));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                        rhandor.stopping_time_neutral_patrol[i] = EditorGUILayout.FloatField(rhandor.stopping_time_neutral_patrol[i], GUILayout.Width(75));
                        if (GUILayout.Button("Reset", GUILayout.Width(55)))
                            rhandor.stopping_time_neutral_patrol[i] = 0.0f;

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }           
        }

        // ---- Alert patrol editor information ---
        alert_expanded = EditorGUILayout.Foldout(alert_expanded, "Alert patrol");
        if (alert_expanded)
        {
            rhandor.static_alert_path = EditorGUILayout.Toggle("Static", rhandor.static_alert_path);
            if (rhandor.static_alert_path)
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

                if (rhandor.alert_path_loop)
                    EditorGUILayout.LabelField("Waypoints: " + ((2 * path_alert_positions.Length) - 2));
                else
                    EditorGUILayout.LabelField("Waypoints: " + path_alert_positions.Length);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                rhandor.alert_speed = EditorGUILayout.FloatField("Alert speed", rhandor.alert_speed, GUILayout.Width(160));
                rhandor.spotted_speed = EditorGUILayout.FloatField("Spotted speed", rhandor.spotted_speed, GUILayout.Width(160));
                EditorGUILayout.EndHorizontal();
                if (path_alert_positions.Length > 1)
                    rhandor.alert_path_loop = EditorGUILayout.Toggle("Loop", rhandor.alert_path_loop);
                else
                    rhandor.alert_path_loop = false;
            }            

            EditorGUILayout.Space();


            if (rhandor.static_neutral_path)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint 1:", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", rhandor.transform.position, GUILayout.Width(240));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < path_alert_positions.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                    EditorGUILayout.Vector3Field("", path_alert_positions[i], GUILayout.Width(240));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                    rhandor.stopping_time_alert_patrol[i] = EditorGUILayout.FloatField(rhandor.stopping_time_alert_patrol[i], GUILayout.Width(75));
                    if (GUILayout.Button("Reset", GUILayout.Width(55)))
                        rhandor.stopping_time_neutral_patrol[i] = 0.0f;
                    EditorGUILayout.EndHorizontal();
                }

                if (rhandor.alert_path_loop)
                {
                    for (int i = path_alert_positions.Length - 2; i > 0; --i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                        EditorGUILayout.Vector3Field("", path_alert_positions[i], GUILayout.Width(240));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                        rhandor.stopping_time_alert_patrol[i] = EditorGUILayout.FloatField(rhandor.stopping_time_alert_patrol[i], GUILayout.Width(75));
                        if (GUILayout.Button("Reset", GUILayout.Width(55)))
                            rhandor.stopping_time_alert_patrol[i] = 0.0f;

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        EditorUtility.SetDirty(rhandor);
    }
}
