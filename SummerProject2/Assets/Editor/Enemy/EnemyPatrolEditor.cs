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

        //Vector3 view_angle_A = item.DirectionFromAngle(-item.view_angle / 2, false);
        //Vector3 view_angle_B = item.DirectionFromAngle(item.view_angle / 2, false);

        //Handles.DrawLine(item.transform.position, item.transform.position + view_angle_A * item.view_radius);
        //Handles.DrawLine(item.transform.position, item.transform.position + view_angle_B * item.view_radius);

        //Handles.color = Color.red;

        //foreach (Transform visible_target in item.visible_targets)
        //{
        //    Handles.DrawLine(item.transform.position, visible_target.position);
        //}

    }


}
