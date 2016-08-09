using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float speed_camera;
    public bool disable_camera_movement_mouse;
    public float zone_for_displacement; // From 0 to 1. For instance, 0.25 means 25% of some length (width or height screen dimensions)
    private Vector2 edge_offset;        // Zone where camera moves will occur when active.
    private Vector3 position_target;
    private float z_correction;         // Z_correction is used for center the different players due to camera inclination.
    private bool smooth_transition;     // When true, a camera transition is occurring.

    // Use this for initialization
    void Start ()
    {
        disable_camera_movement_mouse = true;
        smooth_transition = false;
        speed_camera = 10;
        zone_for_displacement = 0.1f;           // 10% of Width and Height Screen
        edge_offset.Set(zone_for_displacement * Screen.width, zone_for_displacement * Screen.height);
        z_correction = transform.position.y / Mathf.Tan(transform.eulerAngles.x * Mathf.PI / 180);
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckingCameraMovement();
        if (smooth_transition)
            MoveCamera();
    }

    void CheckingCameraMovement()
    {
        Vector3 mouse_position = Input.mousePosition;
        Vector3 pos = transform.position;

        if(!disable_camera_movement_mouse)
        {
            if (mouse_position.x < edge_offset.x)
                pos.Set(pos.x + (-speed_camera * Time.deltaTime), pos.y, pos.z);
            else if (mouse_position.x > Screen.width - edge_offset.x)
                pos.Set(pos.x + (speed_camera * Time.deltaTime), pos.y, pos.z);

            if (mouse_position.y < edge_offset.y)
                pos.Set(pos.x, pos.y, pos.z + (-speed_camera * Time.deltaTime));
            else if (mouse_position.y > Screen.height - edge_offset.y)
                pos.Set(pos.x, pos.y, pos.z + (speed_camera * Time.deltaTime));
        }        

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            float h_dis = Input.GetAxis("CameraHorizontal");
            float v_dis = Input.GetAxis("CameraVertical");
            pos.Set(pos.x + (h_dis * speed_camera * Time.deltaTime), pos.y, pos.z + (v_dis * speed_camera * Time.deltaTime));
        }

        // Updating position
        transform.position = pos;

    }

    public void MoveCameraTo(Vector3 new_pos)
    {
        position_target = new_pos;
        smooth_transition = true;        
    }

    private void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, position_target, 0.5f);
        float minimun_distance = 0.1f;

        if (Vector3.Distance(transform.position, position_target) < minimun_distance)
            smooth_transition = false;
    }
}
