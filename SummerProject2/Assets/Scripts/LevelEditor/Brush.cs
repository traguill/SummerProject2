using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brush : MonoBehaviour
{
    public GameObject obj;
    public float height;
    public string folder_name;
    public string object_name;

    public bool continuous_painting = true;

    [HideInInspector]
    public int id; //List id

    [HideInInspector]
    public List<Vector3> obj_positions = new List<Vector3>();	
}
