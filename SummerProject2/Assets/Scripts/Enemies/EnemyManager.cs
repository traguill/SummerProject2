using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{ 
    [HideInInspector] public GameObject[] enemies;
    [HideInInspector] public GameObject[] players;  // For EnemyViewField
    [HideInInspector] public List<GameObject> list_of_spotted_elements;

    public bool fow_disabled = false; //If the fog of war is disabled the enemies are always visible

    [HideInInspector] public BarionController barion; //Specific reference to barion, for interaction with corpses

    [HideInInspector] public bool god_mode = false; //Only used by console. For read only.

    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag(Tags.player);
        enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        barion = GameObject.Find("Barion").GetComponent<BarionController>();
        list_of_spotted_elements = new List<GameObject>();
    }

	// Use this for initialization
	void Start ()
    { }
	
	// Update is called once per frame
	void Update ()
    { }

    /// <summary>
    /// IsElementAlreadyIdentify checks for an element (corpse, portals, etc) that it has already
    /// been spotted. So, the enemeis won't react again to the same element.
    /// </summary>
    /// <param name="element_to_check"></param>
    /// <returns>True if it has already been identified. False otherwise </returns>
    public bool IsElementAlreadyIdentify(GameObject element_to_check)
    {
        return list_of_spotted_elements.Contains(element_to_check);
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
                enemy_to_destroy.layer = LayerMask.NameToLayer(Layers.corpse);
                enemy_to_destroy.GetComponent<RhandorController>().Dead();
                break;
        }        
    }
}
