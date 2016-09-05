using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorInformationEditor : Editor
{
    private RhandorController rhandor;
    private IRhandorStates state;
    private float ground_level;     // ground correction for proper visualization of patrols
    private Vector3[] path_neutral_positions, path_alert_positions;
    private Vector3 initial_position;  
    private bool neutral_expanded = false, alert_expanded = false;

    private GameObject old_neutral_path, old_alert_path;
    

    void OnEnable()
    {
        rhandor = target as RhandorController;

        ground_level = rhandor.transform.position.y - (rhandor.transform.localScale.y / 2);
        initial_position = new Vector3(rhandor.initial_position.x, ground_level, rhandor.initial_position.z);

        LoadNeutralPatrol();
        LoadAlertPatrol();
        Undo.RecordObject(rhandor, "ShowInspectorPatrolControls");
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
        ShowSupportCall();     
    }

    /// <summary>
    /// Adapts the two arrays that control the stopping times of the enemies to allocate 
    /// the different points that the neutral and the alert paths contain.
    /// </summary>
    private void CheckArrayChanges()
    {
        if(!rhandor.static_neutral)
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

        if (!rhandor.static_alert)
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
        if(rhandor.static_neutral)
        {
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 1, 1, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(initial_position);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint");
            Handles.EndGUI();
            // Cones to indicate positions
            Handles.color = Color.white;
            Handles.ConeCap(0, initial_position, Quaternion.Euler(-90, 0, 0), 1);
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

            Handles.color = Color.white;
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
                Handles.color = Color.white;
                Handles.ConeCap(0, path_neutral_positions[i], Quaternion.Euler(-90, 0, 0), 1);
            }
        }

        // ---------------- For alert patrols ----------------
        if (rhandor.static_alert)
        {    
            // Label indicating the Waypoint number
            Handles.BeginGUI();
            GUI.color = new Color(1, 0, 0, 0.75f);
            Vector2 gui_point = HandleUtility.WorldToGUIPoint(initial_position);
            Rect rect = new Rect(gui_point.x - 40.0f, gui_point.y - 40.0f, 80.0f, 20.0f);
            GUI.Box(rect, "Waypoint");            
            Handles.EndGUI();

            // Cones to indicate positions
            Handles.color = Color.red;
            Handles.ConeCap(0, initial_position, Quaternion.Euler(-90, 0, 0), 1);
        }
        else
        {
            Vector3[] line_segments = new Vector3[path_alert_positions.Length * 2];
            int point_index = 0;

            for (int i = 0; i < path_alert_positions.Length - 1; i++)
            {
                line_segments[point_index++] = path_alert_positions[i];
                line_segments[point_index++] = path_alert_positions[i + 1];
            }

            // We close the alert patrol loop
            if (!rhandor.alert_path_loop)
            {
                line_segments[point_index++] = path_alert_positions[path_alert_positions.Length - 1];
                line_segments[point_index] = path_alert_positions[0];
            }

            Handles.color = Color.red;
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
                Handles.color = Color.red;
                Handles.ConeCap(1, path_alert_positions[i], Quaternion.Euler(-90, 0, 0), 1);
            }
        }            
    }

    private void ShowSupportCall()
    {
        Handles.color = Color.blue;
        Handles.DrawWireArc(rhandor.transform.position, Vector3.up, Vector3.forward, 360, rhandor.ask_for_help_radius);
    }

    private void ShowInspectorPatrolControls()
    {
        //EditorUtility.SetDirty(rhandor);   
        Undo.RecordObject(rhandor, "ShowInspectorPatrolControls");

        // ---- Neutral patrol editor information ---
        neutral_expanded = EditorGUILayout.Foldout(neutral_expanded, "Neutral patrol");
        if (neutral_expanded)
        {
            // Patrols attached as GameObjects and number of waypoints
            rhandor.static_neutral = EditorGUILayout.Toggle("Static", rhandor.static_neutral);
            if (rhandor.static_neutral)
            {
                EditorGUILayout.LabelField("Waypoints: 1");
                rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                rhandor.neutral_path = EditorGUILayout.ObjectField("Path", rhandor.neutral_path, typeof(GameObject), true) as GameObject;
                if(rhandor.neutral_path != old_neutral_path)
                {
                    LoadNeutralPatrol();
                    old_neutral_path = rhandor.neutral_path;
                }

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

            if (rhandor.static_neutral)
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
            rhandor.static_alert = EditorGUILayout.Toggle("Static", rhandor.static_alert);
            if (rhandor.static_alert)
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
                    LoadAlertPatrol();
                    old_alert_path = rhandor.alert_path;
                }

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


            if (rhandor.static_alert)
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

        // Support information
        EditorGUILayout.LabelField("--- Support parameters ---");
        rhandor.max_num_of_helpers = EditorGUILayout.IntField("Nº support enemies", rhandor.max_num_of_helpers, GUILayout.Width(175));
        if (rhandor.max_num_of_helpers < 0) rhandor.max_num_of_helpers = 0;
        rhandor.ask_for_help_radius = EditorGUILayout.FloatField("Radius call", rhandor.ask_for_help_radius);
        if (rhandor.ask_for_help_radius < 0.0f) rhandor.ask_for_help_radius = 0.0f;
    }

    private void LoadNeutralPatrol()
    {
        // Patrols initialization
        if (!rhandor.static_neutral)
        {
            if(rhandor.neutral_path != null)
            {
                // ---- Neutral patrol initialization for editor ----
                Transform[] path = rhandor.neutral_path.transform.getChilds();
                if(path.Length > 1)
                {
                    path_neutral_positions = new Vector3[path.Length];

                    for (int i = 0; i < path.Length; ++i)
                    {
                        path_neutral_positions[i] = path[i].transform.position;
                        path_neutral_positions[i].y = ground_level;
                    }
                        

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
                else
                    Debug.Log("Error loading NEUTRAL PATROL: The patrol must contain more than one waypoint.");               
            }
            else
                Debug.Log("Error loading NEUTRAL PATROL: There is no GameObject attached!");
        }
    }

    private void LoadAlertPatrol()
    {
        // Patrols initialization        
        if (!rhandor.static_alert)
        {
            if (rhandor.alert_path != null)
            {
                // ---- Alert patrol initialization for editor ----
                Transform[] path = rhandor.alert_path.transform.getChilds();
                if (path.Length > 1)
                {
                    path_alert_positions = new Vector3[path.Length];

                    for (int i = 0; i < path.Length; ++i)
                    {
                        path_alert_positions[i] = path[i].transform.position;
                        path_alert_positions[i].y = ground_level;
                    }                        

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
                else
                    Debug.Log("Error loading ALERT PATROL: The patrol must contain more than one waypoint.");
            }
            else
                Debug.Log("Error loading ALERT PATROL: There is no GameObject attached!");
        }
    }
}
