using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.green;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.view_radius);
        Vector3 view_angle_A = fov.DirectionFromAngle(-fov.view_angle / 2, false);
        Vector3 view_angle_B = fov.DirectionFromAngle(fov.view_angle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + view_angle_A * fov.view_radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + view_angle_B * fov.view_radius);

        foreach(Transform visible_target in fov.visible_targets)
        {
            Handles.DrawLine(fov.transform.position, visible_target.position);
        }
    }
}
