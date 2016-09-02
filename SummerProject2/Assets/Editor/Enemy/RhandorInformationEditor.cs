using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RhandorController))]
public class RhandorInformationEditor : Editor
{
    private RhandorController rhandor;
    private RhandorController rhandor_linked;   // For shynchronous patrols.
    private bool sync_is_assigned;
    private IRhandorStates state;

    private GameObject old_neutral_path, old_alert_path;
    private GameObject old_sync_gameobject;

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
        Undo.RecordObject(rhandor, "PatrolInspectorInfo");
        Undo.RecordObject(rhandor, "SupportPatrolInspector");

        PatrolInspectorInfo(rhandor.neutral_patrol, PATROL_TYPE.NEUTRAL);
        PatrolInspectorInfo(rhandor.alert_patrol, PATROL_TYPE.ALERT);
        
        SupportPatrolInspector();
    }

    private void PatrolInspectorInfo(Patrol patrol, PATROL_TYPE type)
    {
        // ---- Patrol editor information ---

        // Info will be hide or expanded depending on expanded boolean.
        switch (type)
        {
            case (PATROL_TYPE.NEUTRAL):
                patrol.expanded = EditorGUILayout.Foldout(patrol.expanded, "Neutral patrol");
                break;
            case (PATROL_TYPE.ALERT):
                patrol.expanded = EditorGUILayout.Foldout(patrol.expanded, "Alert patrol");
                break;
        }

        if (patrol.expanded)
        {
            // Patrols attached as GameObjects and number of waypoints
            patrol.static_patrol = EditorGUILayout.Toggle("Static", patrol.static_patrol);
            if (patrol.static_patrol)
            {
                EditorGUILayout.LabelField("Waypoints: 1");
                switch (type)
                {
                    case (PATROL_TYPE.NEUTRAL):
                        rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
                        break;
                    case (PATROL_TYPE.ALERT):
                        EditorGUILayout.BeginHorizontal();
                        rhandor.alert_speed = EditorGUILayout.FloatField("Alert speed", rhandor.alert_speed, GUILayout.Width(160));
                        rhandor.spotted_speed = EditorGUILayout.FloatField("Spotted speed", rhandor.spotted_speed, GUILayout.Width(160));
                        EditorGUILayout.EndHorizontal();
                        break;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                patrol.path_attached = EditorGUILayout.ObjectField("Path", patrol.path_attached, typeof(GameObject), true) as GameObject;
                switch (type)
                {
                    case (PATROL_TYPE.NEUTRAL):
                        {
                            if (old_neutral_path != patrol.path_attached)
                            {
                                rhandor.LoadNeutralPatrol();
                                old_neutral_path = patrol.path_attached;
                            }
                            break;
                        }
                    case (PATROL_TYPE.ALERT):
                        {
                            if (old_alert_path != patrol.path_attached)
                            {
                                rhandor.LoadAlertPatrol();
                                old_alert_path = patrol.path_attached;
                            }
                            break;
                        }
                }

                if (patrol.loop)
                    EditorGUILayout.LabelField("Waypoints: " + ((2 * patrol.Length) - 2));
                else
                    EditorGUILayout.LabelField("Waypoints: " + patrol.Length);
                EditorGUILayout.EndHorizontal();

                // Speeds information and patrol loop option
                EditorGUILayout.BeginHorizontal();
                switch (type)
                {
                    case (PATROL_TYPE.NEUTRAL):
                        rhandor.patrol_speed = EditorGUILayout.FloatField("Patrol speed", rhandor.patrol_speed, GUILayout.Width(160));
                        if (patrol.Length > 2)
                            patrol.loop = EditorGUILayout.Toggle("Loop", patrol.loop);
                        else
                            patrol.loop = false;
                        EditorGUILayout.EndHorizontal();
                        break;
                    case (PATROL_TYPE.ALERT):
                        rhandor.alert_speed = EditorGUILayout.FloatField("Alert speed", rhandor.alert_speed, GUILayout.Width(160));
                        rhandor.spotted_speed = EditorGUILayout.FloatField("Spotted speed", rhandor.spotted_speed, GUILayout.Width(160));
                        EditorGUILayout.EndHorizontal();
                        if (patrol.Length > 2)
                            patrol.loop = EditorGUILayout.Toggle("Loop", patrol.loop);
                        else
                            patrol.loop = false;
                        break;
                }

                // Synchronous activity
                if (patrol.is_synchronized = EditorGUILayout.Toggle("Patrol synchronized", patrol.is_synchronized))
                {
                    patrol.synchronized_Rhandor = EditorGUILayout.ObjectField("Synchronized Rhandor", patrol.synchronized_Rhandor, typeof(GameObject), true) as GameObject;

                    if (patrol.synchronized_Rhandor != null)
                    {
                        if (patrol.synchronized_Rhandor == rhandor.gameObject || patrol.synchronized_Rhandor.GetComponent<RhandorController>() == null)
                        {
                            Debug.Log("You cannot synchronize this Rhandor itself or " + patrol.synchronized_Rhandor + "is not a Rhandor!");
                            patrol.synchronized_Rhandor = null;
                        }
                    }
                }
                else
                {
                    patrol.synchronized_Rhandor = null;
                }
            }                 

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (patrol.static_patrol)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Waypoint 1:", GUILayout.Width(80));
                EditorGUILayout.Vector3Field("", rhandor.transform.position, GUILayout.Width(240));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < patrol.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                    EditorGUILayout.Vector3Field("", patrol.path[i], GUILayout.Width(240));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                    patrol.stop_times[i] = EditorGUILayout.FloatField(patrol.stop_times[i], GUILayout.Width(75));
                    if (GUILayout.Button("Reset", GUILayout.MaxHeight(15), GUILayout.MaxWidth(55)))
                        patrol.stop_times[i] = 0.0f;
                    EditorGUILayout.EndHorizontal();

                    if (patrol.is_synchronized)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (patrol.trigger_movement[i] = EditorGUILayout.Toggle("Trigger move", patrol.trigger_movement[i]))
                            CheckOneOptionTrigger(patrol, i);
                        if (patrol.recieve_trigger[i] = EditorGUILayout.Toggle("Recieve trigger", patrol.recieve_trigger[i]))
                            CheckOneOptionReciever(patrol, i);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                }

                if (patrol.loop)
                {
                    for (int i = patrol.Length - 2; i > 0; --i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Waypoint " + (i + 1).ToString() + ":", GUILayout.Width(80));
                        EditorGUILayout.Vector3Field("", patrol.path[i], GUILayout.Width(240));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Stop time", GUILayout.Width(75));
                        patrol.stop_times[i] = EditorGUILayout.FloatField(patrol.stop_times[i], GUILayout.Width(75));
                        if (GUILayout.Button("Reset", GUILayout.Width(55)))
                            patrol.stop_times[i] = 0.0f;
                        EditorGUILayout.EndHorizontal();

                        if (patrol.is_synchronized)
                        {
                            EditorGUILayout.BeginHorizontal();
                            patrol.trigger_movement[i] = EditorGUILayout.Toggle("Trigger move", patrol.trigger_movement[i]);
                            patrol.recieve_trigger[i] = EditorGUILayout.Toggle("Recieve trigger", patrol.recieve_trigger[i]);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                        }
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

// --------------------------------------------------------------------------
// ------------------- CHECKING METHODS FOR INSPECTOR -----------------------
// --------------------------------------------------------------------------

    private void CheckOneOptionTrigger(Patrol patrol, int index_to_check)
    {
        for (int i = 0; i < patrol.trigger_movement.Length; ++i)
        {
            if (patrol.trigger_movement[i] && i != index_to_check)
            {
                patrol.trigger_movement[i] = false;
                break;
            }
        }
    }

    private void CheckOneOptionReciever(Patrol patrol, int index_to_check)
    {
        for (int i = 0; i < patrol.recieve_trigger.Length; ++i)
        {
            if (patrol.recieve_trigger[i] && i != index_to_check)
            {
                patrol.recieve_trigger[i] = false;
                break;
            }
        }
    }

}


