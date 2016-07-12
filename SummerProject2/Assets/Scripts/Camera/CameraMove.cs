using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float speed_camera;
    public bool disable_camera_movement_mouse;
    public float zone_for_displacement; // From 0 to 1. For instance, 0.25 means 25% of some lenght (width or height screen dimensions)
    private Vector2 edge_offset;
    private Vector3 position_target;
    private float z_correction; // Z_correction is used for center the different players due to camera inclination.
    private bool smooth_transition;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    // Use this for initialization
    void Start ()
    {
        disable_camera_movement_mouse = true;
        speed_camera = 10;
        zone_for_displacement = 0.1f;   // 10%
        edge_offset.Set(zone_for_displacement * Screen.width, zone_for_displacement * Screen.height);
        z_correction = transform.position.y / Mathf.Tan(transform.eulerAngles.x * Mathf.PI / 180);
    }
	
	// Update is called once per frame
	void Update ()
    {
        checkingCameraMovement();
        if (smooth_transition)
            moveCamera();
    }

    void checkingCameraMovement()
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

        if (Input.GetKey(KeyCode.Alpha1))
        {
            smooth_transition = true;
            position_target.Set(player1.transform.position.x, pos.y, player1.transform.position.z - z_correction);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            smooth_transition = true;
            position_target.Set(player2.transform.position.x, pos.y, player2.transform.position.z - z_correction);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            smooth_transition = true;
            position_target.Set(player3.transform.position.x, pos.y, player3.transform.position.z - z_correction);
        }

        //if (Input.GetKey(KeyCode.Alpha4))
        //{
        //    smooth_transition = true;
        //    position_target.Set(player4.transform.position.x, pos.y, player4.transform.position.z - z_correction);
        //}
    }

    void moveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, position_target, 0.5f);
        float minimun_distance = 0.1f;

        if (Vector3.Distance(transform.position, position_target) < minimun_distance)
            smooth_transition = false;
    }
}
