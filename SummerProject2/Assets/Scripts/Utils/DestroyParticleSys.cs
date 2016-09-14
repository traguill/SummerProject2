using UnityEngine;
using System.Collections;

public class DestroyParticleSys : MonoBehaviour {

    public ParticleSystem particle;
	
	// Update is called once per frame
	void Update () 
    {
        if (!particle.IsAlive())
            Destroy(gameObject);
	}
}
