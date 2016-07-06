using UnityEngine;
using System.Collections;

public class DebugController : MonoBehaviour 
{
    public float speed = 6;

    Rigidbody rigidbody;
    Camera view_camera;
    Vector3 velocity;
	// Use this for initialization
	void Start () 
    {
        rigidbody = GetComponent<Rigidbody>();
        view_camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 mouse_position = view_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, view_camera.transform.position.y));
        transform.LookAt(mouse_position + Vector3.up * transform.position.y);

        velocity = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A)) velocity.x -= 1 * speed;
        if (Input.GetKey(KeyCode.D)) velocity.x += 1 * speed;
        if (Input.GetKey(KeyCode.W)) velocity.z += 1 * speed;
        if (Input.GetKey(KeyCode.S)) velocity.z -= 1 * speed;
	}

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
