using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    GameObject[] enemies;
    GameObject[] players;

    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        players = GameObject.FindGameObjectsWithTag(Tags.player);
    }

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        SetVisible();
	}

    /// <summary>
    /// Sets the enemies visible or not depending of the fog of war.
    /// </summary>
    private void SetVisible()
    {
        List<Transform> visible_enemies = new List<Transform>();

        foreach(GameObject player in players)
        {
            foreach(Transform target in player.GetComponent<FieldOfView>().visible_targets)
            {
                visible_enemies.Add(target);
            }
        }

        //Set all to invisble
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().is_visible = false;
        }
        //Set only the visible enemies to visible
        foreach(Transform visible_enemy in visible_enemies)
        {
            visible_enemy.gameObject.GetComponent<EnemyController>().is_visible = true;
        }
    }
}
