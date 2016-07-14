using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    private EnemyFieldView[] enemy_field_view;
    private ScreenFader screen_fader;

      // Awake is called before any start function
    void Awake()
    {
        screen_fader = GameObject.FindGameObjectWithTag(Tags.screen_fader).GetComponent<ScreenFader>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
        enemy_field_view = new EnemyFieldView[enemies.Length];
        
        for(int i = 0; i < enemies.Length; ++i)
        {
            enemy_field_view[i] = enemies[i].GetComponent<EnemyFieldView>();
        }
    }

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < enemy_field_view.Length; ++i)
        {
            if(enemy_field_view[i].visible_targets.Count > 0)
            {
                screen_fader.EndScene(0);
            }            
        }
    }
}
