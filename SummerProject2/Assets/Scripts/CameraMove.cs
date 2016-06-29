using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float speed_camera;

	// Use this for initialization
	void Start ()
    {
        speed_camera = 10;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float h_dis = Input.GetAxis("CameraHorizontal");
        float v_dis = Input.GetAxis("CameraVertical");
        Vector3 pos = transform.position;
        Vector3 new_pos = transform.position;
        
        new_pos.Set(pos.x + (h_dis * speed_camera * Time.deltaTime), pos.y, pos.z + (v_dis * speed_camera * Time.deltaTime));
        transform.position = new_pos;
    }
}
