using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [HideInInspector] public GameObject[] enemies;
    [HideInInspector] public GameObject[] players;

    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        players = GameObject.FindGameObjectsWithTag(Tags.player);
    }

	// Use this for initialization
	void Start ()
    { }
	
	// Update is called once per frame
	void Update ()
    { }

    /// <summary>
    /// Kill an enemy.
    /// </summary>
    public void KillEnemy(GameObject EnemyToKill)
    {
        EnemyToKill.GetComponent<RhandorController>().Dead();
    }
}
