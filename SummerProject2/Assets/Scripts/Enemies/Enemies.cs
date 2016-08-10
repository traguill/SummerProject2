using UnityEngine;
using System.Collections;

public enum ENEMY_TYPES
{
    RHANDOR,
    CAMERA
}

public class Enemies : MonoBehaviour
{
    [HideInInspector] public ENEMY_TYPES type;

    void Start()
    { }

    void Update()
    { }
}
