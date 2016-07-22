using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*  Handles player selection and selectable objects selection.
 *  It's the main system of all the selection in the game
 */

public class UnitSelection : MonoBehaviour
{
    //Selection rectangle
    public Color sel_rect_color = new Color (0.8f, 0.8f, 0.95f, 0.25f);
    public Color sel_border_color = new Color (0.8f, 0.8f, 0.95f);

    bool is_selecting = false;
    bool is_dragging = false;

    Vector3 mouse_position_begin; //World coordinates

    //Masks
    int floor_mask;
    int player_mask;
    int selectable_object_mask;

    GameObject[] selectable_units;
    List<GameObject> units_selected;

    GameObject object_selected; //Save if an object is selected to NOT update player selection

    void Start()
    {
        floor_mask = LayerMask.GetMask("Floor");
        player_mask = LayerMask.GetMask("Player");
        selectable_object_mask = LayerMask.GetMask("SelectableObject");

        selectable_units = GameObject.FindGameObjectsWithTag("Player");

        units_selected = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        SelectObjects();

        if(object_selected == null) //No object is currently selected
            UpdatePlayerSelection();
        
	}

    void OnGUI()
    {
        if(is_selecting)
        {
            Vector3 origin = Camera.main.WorldToScreenPoint(mouse_position_begin);

            Rect rect = DrawRect.GetScreenRect(origin, Input.mousePosition);
            DrawRect.DrawScreenRect(rect, sel_rect_color);
            DrawRect.DrawScreenRectBorder(rect, 2, sel_border_color);
        }
    }


    private bool IsSelected(GameObject game_object)
    {
        if (!is_selecting)
            return false;

        Vector3 mouse_origin_position = Camera.main.WorldToScreenPoint(mouse_position_begin);
        Bounds viewport_bounds = DrawRect.GetViewportBounds(Camera.main, mouse_origin_position, Input.mousePosition);

        return viewport_bounds.Contains(Camera.main.WorldToViewportPoint(game_object.transform.position));
    }
    
    /// <summary>
    /// Returns true if a Player is currently selected.
    /// </summary>
    public bool IsPlayerSelected(GameObject game_object)
    {
        return units_selected.Find(obj => obj.name == game_object.name);
    }

    private void UpdatePlayerSelection()
    {
        //BUTTON_DOWN
        if (Input.GetMouseButtonDown(0))
        {
            is_selecting = true;

            //Remove previous selected units if key combination is not pressed
            if (Input.GetAxis("MultipleSelection") == 0)
            {
                units_selected.Clear();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100, floor_mask);
            mouse_position_begin = hit.point;
        }

        //BUTTON_UP
        if (Input.GetMouseButtonUp(0))
        {
            if (is_dragging == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, player_mask))
                {
                    //Add and Normal
                    if (Input.GetAxis("MultipleSelection") >= 0)
                    {
                        if (units_selected.Find(obj => obj.name == hit.transform.gameObject.name) == false)
                            units_selected.Add(hit.transform.gameObject);
                    }

                    //Remove
                    if (Input.GetAxis("MultipleSelection") < 0)
                    {
                        units_selected.Remove(hit.transform.gameObject);
                    }

                }
            }

            is_selecting = false;
            is_dragging = false;
        }


        //Difference between click and drag(is_selecting)
        if (is_selecting)
        {
            if (is_dragging)
            {
                foreach (GameObject obj in selectable_units)
                {
                    if (IsSelected(obj) == true)
                    {
                        if (Input.GetAxis("MultipleSelection") >= 0)
                        {
                            if (units_selected.Find(game_obj => game_obj.name == obj.transform.gameObject.name) == false)
                            {
                                units_selected.Add(obj);
                            }
                        }
                        else
                        {
                            units_selected.Remove(obj);
                        }
                    }
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 100, floor_mask);
                if (hit.point != mouse_position_begin)
                    is_dragging = true;
            }

        }
    }

    private void SelectObjects()
    {
        //Button down -> check selectableobject clicked
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, selectable_object_mask))
            {
                object_selected = hit.transform.gameObject;

                //Tell object that is being selected (all types of objects here)
                if (object_selected.tag == Tags.box)
                    object_selected.GetComponent<BoxController>().Selected();
     
            }
            else
            {
                object_selected = null;
            }
        }

        
    }
}
