using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyPatrol))]
public class EnemyPatrolEditor : Editor
{
    [ExecuteInEditMode]
    void OnSceneGUI()
    {
        EnemyPatrol item = (EnemyPatrol)target;
        Transform[] path = item.assigned_path.transform.getChilds();

        Vector3[] lineSegments = new Vector3[path.Length * 2];
        int pointIndex = 0;

        for (int i = 0; i < path.Length - 1; i++)
        {
            lineSegments[pointIndex++] = path[i].transform.position;
            lineSegments[pointIndex++] = path[i + 1].transform.position;
        }

        Handles.DrawDottedLines(lineSegments, 1);

    }
}
