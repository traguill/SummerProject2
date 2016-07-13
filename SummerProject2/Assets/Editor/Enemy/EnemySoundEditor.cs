using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(EnemyFootsteps))]
public class EnemySoundEditor : Editor {

	void OnSceneGUI()
    {
        EnemyFootsteps item = (EnemyFootsteps)target;

        Handles.color = Color.blue;

        Handles.DrawWireArc(item.transform.position, Vector3.up, Vector3.forward, 360, item.sound_radius);
    }
}
