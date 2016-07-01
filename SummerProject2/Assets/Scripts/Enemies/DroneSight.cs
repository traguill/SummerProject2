using UnityEngine;
using System.Collections;

public class DroneSight : MonoBehaviour
{

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            print("Drone has detected: " + other.name);
        }
    }
}
