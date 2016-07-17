using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyPatrol))]
public class EnemyPatrolEditor : Editor
{
    private Vector3[] path_positions;

    void OnEnable()
    {
        EnemyPatrol item = target as EnemyPatrol;
        Transform[] path = item.assigned_path.transform.getChilds();
        path_positions = new Vector3[path.Length];

        for (int i = 0; i < path.Length; ++i)
        {
            path_positions[i] = path[i].transform.position;
        }
    }

    public override void OnInspectorGUI()
    {
        for (int i = 0; i < path_positions.Length; i++)
        {
            EditorGUILayout.Vector2Field("Point " + (i+1), new Vector2(path_positions[i].x, path_positions[i].z) );
        }
    }

    [ExecuteInEditMode]
    void OnSceneGUI()
    {
       Vector3[] line_segments = new Vector3[path_positions.Length * 2];
        int point_index = 0;

        for (int i = 0; i < path_positions.Length - 1; i++)
        {
            line_segments[point_index++] = path_positions[i];
            line_segments[point_index++] = path_positions[i + 1];
        }

        // We close the patrol loop
        line_segments[point_index++] = path_positions[path_positions.Length - 1];
        line_segments[point_index] = path_positions[0];

        Handles.DrawDottedLines(line_segments, 1.5f);
        Color initial_color = Color.green;
        Color final_color = Color.red;

        for (int i = 0; i < path_positions.Length; i++)
        {
            Handles.color = initial_color;
            initial_color = Color.Lerp(initial_color, final_color, 0.5f);
            Handles.ConeCap(0, path_positions[i], Quaternion.Euler(-90,0,0), 1);
        }
    }
}
