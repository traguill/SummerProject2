using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{ 
    [HideInInspector] public GameObject[] enemies;
    [HideInInspector] public GameObject[] players;

    GameObject rhandor;

    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        players = GameObject.FindGameObjectsWithTag(Tags.player);

        rhandor = GameObject.Find("Rhandor");
    }

	// Use this for initialization
	void Start ()
    { }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            DestroyEnemy(rhandor);
        }
    }

    /// <summary>
    /// Destroy an enemy.
    /// </summary>
    public void DestroyEnemy(GameObject enemy_to_destroy)
    {
        Enemies enemy = enemy_to_destroy.GetComponent<Enemies>();

        switch(enemy.type)
        {
            case (ENEMY_TYPES.RHANDOR):
                enemy_to_destroy.tag = Tags.corpse;
                enemy_to_destroy.GetComponent<RhandorController>().Dead();
                break;
        }        
    }
}
