using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float speed_camera;              // Camera veloctiy for all kind of movement
    private Vector3 target_position;        // Position that camera will reach.
    private bool do_transition;             // When true, a camera transition is occurring, reaching target_position;
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

    // Use this for initialization
    void Start ()
    {
        do_transition = false;
        edge_offset.Set(zone_for_displacement * Screen.width, zone_for_displacement * Screen.height);
        z_correction = CalculateZCorrection(transform.position.y);
        float angle = 90.0f - transform.rotation.eulerAngles.x;
        float center_to_camera_floor = Mathf.Tan(angle * (Mathf.PI / 180)) * transform.position.y;
        scroll_direction = (new Vector3(0, 0, center_to_camera_floor) - new Vector3( 0, transform.position.y, 0)).normalized;
        initial_height = transform.position.y;
        target_position = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckingCameraMovement();

        if (do_transition)
            MoveCamera();
    }

    void CheckingCameraMovement()
    {
        Vector3 mouse_position = Input.mousePosition;
        Vector3 curr_pos = target_position;

        // With mouse on the screen edges
        if(enable_mouse_displacement)
        {
            if (mouse_position.x < edge_offset.x)
            {
                curr_pos.Set(curr_pos.x + (-speed_camera * Time.deltaTime), curr_pos.y, curr_pos.z);
            }                
            else if (mouse_position.x > Screen.width - edge_offset.x)
            {
                curr_pos.Set(curr_pos.x + (speed_camera * Time.deltaTime), curr_pos.y, curr_pos.z);
            }                

            if (mouse_position.y < edge_offset.y)
            {
                curr_pos.Set(curr_pos.x, curr_pos.y, curr_pos.z + (-speed_camera * Time.deltaTime));
            }                
            else if (mouse_position.y > Screen.height - edge_offset.y)
            {
                curr_pos.Set(curr_pos.x, curr_pos.y, curr_pos.z + (speed_camera * Time.deltaTime));
            }                
        }

        // With mouse with arrow keys
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            float h_dis = Input.GetAxis("CameraHorizontal");
            float v_dis = Input.GetAxis("CameraVertical");
            curr_pos.Set(curr_pos.x + (h_dis * speed_camera * Time.deltaTime), curr_pos.y, curr_pos.z + (v_dis * speed_camera * Time.deltaTime));
        }

        // With scroll whell from mouse
        Vector2 mouse_scroll = Input.mouseScrollDelta;  // X for horizontal and Y for vertical movement

        if (mouse_scroll.y != 0) // Zoom in / zoom out 
        {
            float new_height = curr_pos.y + mouse_scroll.y * scroll_direction.y * speed_camera * Time.deltaTime;
            float zcorr = 0.0f; 

            if (new_height > max_camera_height)
            {
                zcorr = CalculateZCorrection(max_camera_height, curr_pos.y);
                curr_pos.Set(curr_pos.x, max_camera_height, curr_pos.z - zcorr);
            }                
            else if (new_height < min_camera_height)
            {
                zcorr = CalculateZCorrection(min_camera_height, curr_pos.y);
                curr_pos.Set(curr_pos.x, min_camera_height, curr_pos.z - zcorr);
            }              
            else
                curr_pos.Set(curr_pos.x, new_height, curr_pos.z + mouse_scroll.y * scroll_direction.z * speed_camera * Time.deltaTime);
        }        
            
        
        if(Input.GetMouseButtonUp(2))
        {
            float zcorr = CalculateZCorrection(initial_height, curr_pos.y);
            curr_pos.Set(curr_pos.x, initial_height, curr_pos.z - zcorr);
        }       

        if(target_position != curr_pos)
        {
            do_transition = true;
            target_position = curr_pos;
        }
    }

    public void MoveCameraTo(Vector3 new_pos)
    {
        target_position = new Vector3(new_pos.x, transform.position.y, new_pos.z - z_correction);
        do_transition = true;        
    }

    private void MoveCamera()
    {        
        float minimun_distance = 0.1f;

        if (Vector3.Distance(transform.position, target_position) < minimun_distance)            
            do_transition = false;
        else
            transform.position = Vector3.Lerp(transform.position, target_position, 0.2f);            
    }

    private float CalculateZCorrection(float height_dest, float actual_height = 0)
    {
        return (height_dest - actual_height) / Mathf.Tan(transform.eulerAngles.x * (Mathf.PI / 180));
    }
}
