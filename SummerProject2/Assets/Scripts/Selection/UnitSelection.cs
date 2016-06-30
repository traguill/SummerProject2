using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSelection : MonoBehaviour
{
    //Selection rectangle
    public Color sel_rect_color = new Color (0.8f, 0.8f, 0.95f, 0.25f);
    public Color sel_border_color = new Color (0.8f, 0.8f, 0.95f);

    bool is_selecting = false;
    Vector3 mouse_position_begin; //World coordinates

    int floor_mask;

    GameObject[] selectable_units;
    List<GameObject> units_selected;

    void Start()
    {
        floor_mask = LayerMask.GetMask("Floor");

        selectable_units = GameObject.FindGameObjectsWithTag("Player");

        units_selected = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetMouseButtonDown(0))
        {
            //Remove previous selected units

            if(units_selected.Count > 0)
            {
                foreach (GameObject obj in units_selected)
                {
                    obj.GetComponent<PlayerController>().is_selected = false;
                }
                units_selected.Clear();
            }
            

            is_selecting = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100, floor_mask);
            mouse_position_begin = hit.point;
        }

        if(Input.GetMouseButtonUp(0))
        {
            is_selecting = false;

            if(units_selected.Count > 0)
            {
                foreach (GameObject obj in units_selected)
                {
                    obj.GetComponent<PlayerController>().is_selected = true;
                }
            }
            
        }

        if(is_selecting)
        {
            foreach (GameObject obj in selectable_units)
            {
                if(IsSelected(obj) == true)
                {
                    obj.GetComponent<PlayerController>().is_selected = true;
                    units_selected.Add(obj);
                }
            }
        }
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

    public bool IsSelected(GameObject game_object)
    {
        if (!is_selecting)
            return false;

        Vector3 mouse_origin_position = Camera.main.WorldToScreenPoint(mouse_position_begin);
        Bounds viewport_bounds = DrawRect.GetViewportBounds(Camera.main, mouse_origin_position, Input.mousePosition);

        return viewport_bounds.Contains(Camera.main.WorldToViewportPoint(game_object.transform.position));
    }
}
