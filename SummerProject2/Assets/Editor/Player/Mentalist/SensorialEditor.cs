using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SensorialAbility))]
public class SensorialEditor : Editor
{

    void OnSceneGUI()
    {
        SensorialAbility item = (SensorialAbility)target;

        Handles.color = Color.yellow;

        Handles.DrawWireArc(item.transform.position, Vector3.up, Vector3.forward, 360, item.detection_radius);
    }
}