using UnityEngine;
using System.Collections;

public class ForwardDirection : MonoBehaviour {

    private RhandorController rhandor;

	// Use this for initialization
	void Awake ()
    {
        rhandor = GetComponentInParent<RhandorController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector3(rhandor.transform.position.x, rhandor.transform.position.y, rhandor.transform.position.z) ;
        
	}
}
