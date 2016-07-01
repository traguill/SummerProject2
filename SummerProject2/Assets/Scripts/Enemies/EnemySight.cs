using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour
{
    public bool debug = false; //Draws vision lines

    public float field_of_view_angle = 110f;

    private SphereCollider range;
    private Vector3 current_direction = new Vector3(0, 0, -1); //Change this for the actual direction

    void Awake()
    {
        range = GetComponent<SphereCollider>();
    }

    void Update()
    {
        

        if(debug)
        {
            //DrawLines
            Vector3 left_line = new Vector3(current_direction.x * range.radius, current_direction.y * range.radius, current_direction.z * range.radius);
            left_line = Quaternion.Euler(0, field_of_view_angle * 0.5f, 0) * left_line;
            left_line += transform.position;

            Debug.DrawLine(transform.position, left_line, Color.green);

            Vector3 right_line = new Vector3(current_direction.x * range.radius, current_direction.y * range.radius, current_direction.z * range.radius);
            right_line = Quaternion.Euler(0, field_of_view_angle * -0.5f, 0) * right_line;
            right_line += transform.position;

            Debug.DrawLine(transform.position, right_line, Color.green);


            //Move enemy
            Vector3 movement = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.A))
            {
                movement.x -= Time.deltaTime * 18;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement.x += Time.deltaTime * 18;
            }

            if (Input.GetKey(KeyCode.W))
            {
                movement.z += Time.deltaTime * 18;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movement.z -= Time.deltaTime * 18;
            }

            transform.position += movement;
        }

    }


    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            Vector3 direction = other.transform.position - transform.position;

            float angle = Vector3.Angle(direction, current_direction);

            if (angle < field_of_view_angle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction.normalized, out hit, range.radius))
                {
                    if(hit.transform.gameObject == other.transform.gameObject)
                    {
                        print("I see " + hit.transform.name);
                    }
                }
            }
        }   
    }
	
}
