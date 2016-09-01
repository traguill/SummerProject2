using UnityEngine;
using System.Collections;

public class InvisibleSphere : MonoBehaviour
{
    public float speed = 10; //Movement speed

    [HideInInspector]public Vector3 direction = new Vector3(); //Direction of movement

    public LayerMask disappear_layer; //Layers that on collision with the sphere this will disappear
    public LayerMask players_layer; //Layers that will make the object invisible on collision with the sphere

	
	// Update is called once per frame
	void Update ()
    {
        transform.position += Time.deltaTime * speed * direction;
	}

    void OnTriggerEnter(Collider col)
    {

            if (players_layer == (players_layer | (1 << col.gameObject.layer))) //Another player has been hit
            {
                col.gameObject.GetComponent<Invisible>().TurnInvisible(); //Turn the player invisible
                Destroy(gameObject);
            }

            if(disappear_layer == (disappear_layer | (1 << col.gameObject.layer))) //Something that makes the sphere disappear has been hit
            {
                Destroy(gameObject);
            }
    }
}
