using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(EnemyFieldView))]
public class EnemyFieldViewEditor : Editor {

	void OnSceneGUI()
    {
        EnemyFieldView item = (EnemyFieldView)target;

        Handles.color = Color.green;
        Handles.DrawWireArc(item.transform.position, Vector3.up, Vector3.forward, 360, item.view_radius);
        Vector3 view_angle_A = item.DirectionFromAngle(-item.view_angle / 2, false);
        Vector3 view_angle_B = item.DirectionFromAngle(item.view_angle / 2, false);

        Handles.DrawLine(item.transform.position, item.transform.position + view_angle_A * item.view_radius);
        Handles.DrawLine(item.transform.position, item.transform.position + view_angle_B * item.view_radius);

        Handles.color = Color.red;

        foreach (Transform visible_target in item.visible_targets)
        {
            Handles.DrawLine(item.transform.position, visible_target.position);
        }

    }
}
