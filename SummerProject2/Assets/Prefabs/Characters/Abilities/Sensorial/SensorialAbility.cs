using UnityEngine;
using System.Collections;

public class SensorialAbility : MonoBehaviour {

    public float life = 0.75f;

    float timer = 0.0f;
	
	// Update is called once per frame
	void Update () 
    {
        timer += Time.deltaTime;

        if (timer >= life)
            Destroy(gameObject);
	}
}
