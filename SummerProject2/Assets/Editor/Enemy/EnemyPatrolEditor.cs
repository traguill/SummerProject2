using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyAController))]
public class EnemyPatrolEditor : Editor
{
    private Vector3[] path_neutral_positions;
    private Vector3[] path_alarm_positions;

    void OnEnable()
    {
        EnemyAController item = target as EnemyAController;

        Transform[] path = item.neutral_path.transform.getChilds();
        path_neutral_positions = new Vector3[path.Length];

        for (int i = 0; i < path.Length; ++i)
        {
            path_neutral_positions[i] = path[i].transform.position;
        }

        //path = item.alarm_path.transform.getChilds();
        path_alarm_positions = new Vector3[path.Length];

        for (int i = 0; i < path.Length; ++i)
        {
            path_alarm_positions[i] = path[i].transform.position;
        }
    }

    // CRZ -> I don't like the layout!
    //public override void OnInspectorGUI()
    //{
    //    for (int i = 0; i < path_positions.Length; i++)
    //    {
    //        EditorGUILayout.Vector2Field("Point " + (i+1), new Vector2(path_positions[i].x, path_positions[i].z) );
    //    }
    //}

    [ExecuteInEditMode]
    void OnSceneGUI()
    {
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
            Handles.ConeCap(0, path_neutral_positions[i], Quaternion.Euler(-90,0,0), 1);
            Handles.Label(path_neutral_positions[i], i.ToString());
        }

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

        Handles.color = Color.red;
        Handles.DrawDottedLines(line_segments, 1.5f);
        
        for (int i = 0; i < path_alarm_positions.Length; i++)
        {
            Handles.ConeCap(0, path_alarm_positions[i], Quaternion.Euler(-90, 0, 0), 1);
            Handles.Label(path_alarm_positions[i], i.ToString());
        }
    }
}
