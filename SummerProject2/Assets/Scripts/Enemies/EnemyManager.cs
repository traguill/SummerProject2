using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{ 
    [HideInInspector] public GameObject[] enemies;
    [HideInInspector] public GameObject[] players; //For enemy view

    public bool fow_disabled = false; //If the fog of war is disabled the enemies are always visible

    [HideInInspector] public BarionController barion; //Specific reference to barion, for interaction with corpses
    public List<GameObject> list_of_corpses, list_of_portals;

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
    /// Is that element already identified? Enemies won't activate the alarm if the element has already been checked.
    /// </summary>
    /// <param name="element_to_check">The corpse to check</param>
    /// <returns>True if the elmenet has been already identified. False, otherwise</returns>
    public bool IsElementAlreadyIdentify(GameObject element_to_check, List<GameObject> list_to_check)
    {
        foreach(GameObject element in list_to_check)
        {
            if (element_to_check == element)
                return true;
        }      
            
        return false;
    }
}
