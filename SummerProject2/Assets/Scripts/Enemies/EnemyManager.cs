using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{ 
    [HideInInspector] public GameObject[] enemies;
    [HideInInspector] public GameObject[] players; //For enemy view

    public bool fow_disabled = false; //If the fog of war is disabled the enemies are always visible

    [HideInInspector] public BarionController barion; //Specific reference to barion, for interaction with corpses
    public List<GameObject> list_of_corpses;

    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        players = GameObject.FindGameObjectsWithTag(Tags.player);

        barion = GameObject.Find("Barion").GetComponent<BarionController>();
    }

	// Use this for initialization
	void Start ()
    { }
	
	// Update is called once per frame
	void Update ()
    { }

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
                enemy_to_destroy.layer = LayerMask.NameToLayer(Layers.corpse);
                enemy_to_destroy.GetComponent<RhandorController>().Dead();
                break;
        }        
    }

    /// <summary>
    /// Is that corpse already identified? Enemies won't activate the alarm if the corpse has already been checked.
    /// </summary>
    /// <param name="corpse_to_check">The corpse to check</param>
    /// <returns>True if the corpse has been already identified. False, otherwise</returns>
    public bool IsCorpseAlreadyIdentify(GameObject corpse_to_check)
    {
        if(corpse_to_check.tag.Equals(Tags.corpse))
        {
            foreach(GameObject corpse in list_of_corpses)
            {
                if (corpse_to_check == corpse)
                    return true;
            }
        }
        else
            Debug.Log("Enemy " + corpse_to_check.name + " is not dead!");
            
        return false;
    }
}
