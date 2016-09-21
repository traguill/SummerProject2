using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float speed_camera;              // Camera veloctiy for all kind of movement
    private Vector3 target_position;        // Position that camera will reach.
    private bool do_translation;            // When true, a camera transition is occurring, reaching target_position keeping on the same height;
    private float z_correction;             // To put elements on the exact center of the screen.

    // Movement by screen edges
    public bool enable_mouse_displacement;  // Zones for displacement active (RTS style)
    public float zone_for_displacement;     // Portion active for zones for displacement. From 0 to 1.
                                            // For instance, 0.25 means 25% of some length (width or height screen dimensions)
    private Vector2 edge_offset;            // Zone in pixels where camera movement will occur when active.

    // Movement by scrolling
    public float max_camera_height;         // Max height when scrolling;
    public float min_camera_height;         // Min height when scrolling;
    private float initial_height;           // When the camera has started.
    private Vector3 scroll_direction;       // Direction of camera when scrolling acts.
    private bool scrolling;

    // Orbital movement
    private Vector3 center_ground;
    private float angle_circle;
    private float radius_circle;
    private bool orbiting;

    private Vector3 fwd, lat;

    // Use this for initialization
    void Start ()
    {
        do_translation = false;       
        z_correction = CalculateZCorrection(transform.position.y);

        edge_offset.Set(zone_for_displacement * Screen.width, zone_for_displacement * Screen.height);

        center_ground = new Vector3(transform.position.x, 0.0f, transform.position.z + z_correction);
        scroll_direction = (transform.position - center_ground).normalized;
        initial_height = transform.position.y;
        scrolling = false;

        fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        lat = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        radius_circle = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(center_ground.x, center_ground.z));
        angle_circle = 270;

        orbiting = false;

        target_position = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckingCameraMovement();

        Debug.DrawLine(transform.position, center_ground);

        if (do_translation)
            MoveCamera();
    }

    void CheckingCameraMovement()
    {
        Vector3 mouse_position = Input.mousePosition;
        Vector3 curr_pos = target_position;

        fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        lat = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        // With mouse on the screen edges
        if (enable_mouse_displacement)
        {
            if (mouse_position.x < edge_offset.x)
            {
                curr_pos = curr_pos - speed_camera * Time.deltaTime * lat;
                //curr_pos.Set(curr_pos.x + (-speed_camera * Time.deltaTime), curr_pos.y, curr_pos.z);
            }                
            else if (mouse_position.x > Screen.width - edge_offset.x)
            {
                curr_pos = curr_pos + speed_camera * Time.deltaTime * lat;
                //curr_pos.Set(curr_pos.x + (speed_camera * Time.deltaTime), curr_pos.y, curr_pos.z);
            }                

            if (mouse_position.y < edge_offset.y)
            {
                curr_pos = curr_pos - speed_camera * Time.deltaTime * fwd;
                //curr_pos.Set(curr_pos.x, curr_pos.y, curr_pos.z + (-speed_camera * Time.deltaTime));
            }                
            else if (mouse_position.y > Screen.height - edge_offset.y)
            {
                curr_pos = curr_pos + speed_camera * Time.deltaTime * fwd;
                //curr_pos.Set(curr_pos.x, curr_pos.y, curr_pos.z + (speed_camera * Time.deltaTime));
            }                
        }

        // With mouse with arrow keys
        if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            float v_dis = Input.GetAxis("CameraVertical");
            curr_pos = curr_pos + (v_dis * speed_camera * Time.deltaTime * fwd);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            float h_dis = Input.GetAxis("CameraHorizontal");
            curr_pos = curr_pos + (h_dis * speed_camera * Time.deltaTime * lat);
        }   

        // With scroll whell from mouse
        Vector2 mouse_scroll = Input.mouseScrollDelta;  // X for horizontal and Y for vertical movement

        if (mouse_scroll.y != 0) // Zoom in / zoom out 
        {
            scrolling = true;

            float new_height = curr_pos.y + (scroll_direction.y * speed_camera * Time.deltaTime * -mouse_scroll.y);

            if (new_height > min_camera_height && new_height < max_camera_height)
                curr_pos = curr_pos + (scroll_direction * speed_camera * Time.deltaTime * -mouse_scroll.y); 
        }

        // Recovering orientation and height
        if (Input.GetMouseButtonUp(2))
        {
            orbiting = true;
            float factor = (initial_height - curr_pos.y) / scroll_direction.y;
            curr_pos = curr_pos + (factor * scroll_direction);

            angle_circle = 270;
            curr_pos.Set(center_ground.x + radius_circle * Mathf.Cos(angle_circle * (Mathf.PI / 180)), curr_pos.y,
                                          center_ground.z + radius_circle * Mathf.Sin(angle_circle * (Mathf.PI / 180)));
        }

        // Orbiting
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
        {
            angle_circle += (Input.GetAxis("MouseX") * 5 * Time.deltaTime);
            curr_pos.Set(center_ground.x + radius_circle * Mathf.Cos(angle_circle * (Mathf.PI / 180)), curr_pos.y,
                                          center_ground.z + radius_circle * Mathf.Sin(angle_circle * (Mathf.PI / 180)));
            orbiting = true;
        }

        if (target_position != curr_pos)
        {
            do_translation = true;
            target_position = curr_pos;
        }
    }

    public void MoveCameraTo(Vector3 new_pos)
    {
        target_position = new Vector3(new_pos.x, transform.position.y, new_pos.z - z_correction);
        do_translation = true;
    }

    private void MoveCamera()
    {        
        float minimun_distance = 0.1f;

        if (Vector3.Distance(transform.position, target_position) < minimun_distance)
        {
            do_translation = false;
            orbiting = false;
            scrolling = false;
        }     
        else
        {
            if (orbiting)
            {
                transform.position = Vector3.Slerp(transform.position, target_position, 0.2f);
                transform.LookAt(center_ground);
            }
            else if(scrolling)
            {
                transform.position = Vector3.Lerp(transform.position, target_position, 0.2f);
            }      
            else
            {
                transform.position = Vector3.Lerp(transform.position, target_position, 0.2f);
                center_ground = transform.position + (Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * radius_circle);
                center_ground.y = 0.0f;
                scroll_direction = (transform.position - center_ground).normalized;
            }   
        }                 
    }

    private float CalculateZCorrection(float height_dest, float actual_height = 0)
    {
        return (height_dest - actual_height) / Mathf.Tan(transform.eulerAngles.x * (Mathf.PI / 180));
    }
}
