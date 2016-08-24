using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour 
{
    //Cursor Textures (i -> idle / p -> pressed)
    public Texture2D idle_i;
    public Texture2D idle_p;
    public Texture2D attack;
    public Texture2D ally_i;
    public Texture2D ally_p; 
    public Texture2D interact_i; 
    public Texture2D interact_p;

    Vector2 hot_spot = Vector2.zero;

    public LayerMask interact_layers;

    PlayersManager players_manager;


	void Awake () 
    {
        players_manager = GameObject.Find("Players").GetComponent<PlayersManager>();
	    Cursor.SetCursor(idle_i, hot_spot, CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Start () 
    {
        StartCoroutine("UpdateCursor", 0.1f);
	}

    IEnumerator UpdateCursor(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);

            bool pressed = false;

            if (Input.GetMouseButton(0))
                pressed = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, interact_layers))
            {
                UpdateState(hit, pressed);
            }
        }
        
    }

    private void UpdateState(RaycastHit hit, bool pressed)
    {
        //Attack (nyx selected and mouse over enemy)
        if(hit.transform.tag == Tags.enemy && players_manager.IsNyxSelected())
        {
            EnemyVisibility enemy = hit.transform.GetComponent<EnemyVisibility>();

            if(enemy != null)
            {
                if(enemy.IsVisible())
                {
                    Cursor.SetCursor(attack, hot_spot, CursorMode.Auto);
                    return;
                }      
            }
            else
            {
                Debug.Log("Enemy: " + hit.transform.name + " doesn't have script EnemyVisibility attached. Cursor State CAN'T update");
            } 
        }


        int layer = hit.transform.gameObject.layer;
        //Interact
        if(layer == LayerMask.NameToLayer(Layers.selectable_object))
        {
            if(pressed)
                Cursor.SetCursor(interact_p, hot_spot, CursorMode.Auto);
            else
                Cursor.SetCursor(interact_i, hot_spot, CursorMode.Auto);

            return;
        }

        //Interact (special cases)

        //Barion with corpses
        if(layer == LayerMask.NameToLayer(Layers.corpse))
        {
            if(players_manager.IsBarionSelected())
            {
                if(pressed)
                    Cursor.SetCursor(interact_p, hot_spot, CursorMode.Auto);
                else
                    Cursor.SetCursor(interact_i, hot_spot, CursorMode.Auto);

                return;
            }
        }

        //Allies
        if(hit.transform.tag == Tags.player)
        {
            if (pressed)
                Cursor.SetCursor(ally_p, hot_spot, CursorMode.Auto);
            else
                Cursor.SetCursor(ally_i, hot_spot, CursorMode.Auto);

            return;
        }


        //Default 
        if(pressed)
            Cursor.SetCursor(idle_p, hot_spot, CursorMode.Auto);
        else
            Cursor.SetCursor(idle_i, hot_spot, CursorMode.Auto);
    }
}
