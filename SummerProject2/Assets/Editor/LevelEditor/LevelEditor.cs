﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class LevelEditor : EditorWindow
{
    GameObject brush_obj = null;

    //Prefabs (brush)
    GameObject floor_prefab = null;
    GameObject wall_prefab = null;

    //Creation height
    float floor_height = -0.05f;
    float wall_height = 0.5f;

    //Brushes
    enum Brushes
    {
        NONE,
        WALL,
        FLOOR
    }

    Brushes brush = Brushes.NONE;

    //Tools
    enum Tools
    {
        NONE,
        PAINT,
        ERASE,
        RECT,
        DROPPER
    }

    Tools tool = Tools.NONE;

    //Options
    bool paint_enable = false;
    bool erase_enable = false;
    bool rect_enable = false;
    bool editor_enable = false;
    bool dropper_enable = false;

    //Containers
    GameObject wall_container;
    GameObject floor_container;

    //Saved data
    List<Vector3> floor_positions = new List<Vector3>();
    List<Vector3> wall_positions = new List<Vector3>();

    [MenuItem("Window/Level Editor")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelEditor));
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate = OnSceneGUI;

        Init();
    }

    private void Init()
    {
        GameObject level = GameObject.Find("Level");
        wall_container = GameObject.Find("Walls");
        floor_container = GameObject.Find("Floors");
        if (level == null)
        {
            level = new GameObject();
            level.name = "Level";
        }

        if (wall_container == null)
        {
            wall_container = new GameObject();
            wall_container.name = "Walls";
            wall_container.transform.SetParent(level.transform);
        }

        if (floor_container == null)
        {
            floor_container = new GameObject();
            floor_container.name = "Floors";
            floor_container.transform.SetParent(level.transform);
        }

        SavePositions();

    }

    //Level editor window Update
    void OnGUI()
    {
        //Editor is enabled?
        editor_enable = EditorGUILayout.Toggle("Editor", editor_enable);

        
        if (editor_enable)
        {
            ToolOptions();
        }

        Options();  
    }

    //SceneView Input manager
    void OnSceneGUI(SceneView sceneView)
    {
        if (!editor_enable)
            return;

        HandleUtility.AddDefaultControl(0); //Disable engine input

        //Shortcuts
        Event e = Event.current;

        //Paint (B)
        if (e.keyCode == KeyCode.B && e.type == EventType.keyDown)
        {
            tool = Tools.PAINT;
            UpdateToolCheck();
            Repaint();
        }

        //Erase (E)
        if (e.keyCode == KeyCode.E && e.type == EventType.keyDown)
        {
            tool = Tools.ERASE;
            UpdateToolCheck();
            Repaint();
        }

        //Rect (R)
        if(e.keyCode == KeyCode.R && e.type == EventType.keyDown)
        {
            tool = Tools.RECT;
            UpdateToolCheck();
            Repaint();
        }

        if(e.keyCode == KeyCode.I && e.type == EventType.keyDown)
        {
            tool = Tools.DROPPER;
            UpdateToolCheck();
            Repaint();
        }

        //Paint or Erase
        if(paint_enable || erase_enable)
        {
            if ((e.type == EventType.mouseDown || e.type == EventType.mouseDrag) && e.button == 0 && e.alt == false)
            {    
                if(paint_enable)
                    CreateObject(e);

                if (erase_enable)
                    EraseObject(e);
            }
        } 
   
        //Draw Rect
        if(rect_enable)
        {
            RectTool(e);
        }

        //Dropper
        if(dropper_enable)
        {
            if(e.type == EventType.mouseDown && e.button == 0 && e.alt == false)
            {
                DropperTool(e);
            }
        }
    }

    //Utils ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Saves all the objects positions created by the tool. Used after when optimize and take appart.
    /// </summary>
    private void SavePositions()
    {
        //Save objects position
        floor_positions.Clear();
        Transform[] childs_floor = floor_container.transform.getChilds();
        for (int i = 0; i < childs_floor.Length; i++)
        {
            floor_positions.Add(childs_floor[i].position);
        }

        wall_positions.Clear();
        Transform[] childs_wall = wall_container.transform.getChilds();
        for (int i = 0; i < childs_wall.Length; i++)
        {
            wall_positions.Add(childs_wall[i].position);
        }
    }

    /// <summary>
    /// Features of the level editor. Update of every option
    /// </summary>
    private void ToolOptions()
    {
        //Tools-------------------------------------------------------------------------------------------------
        GUILayout.Label("Tools", EditorStyles.boldLabel);

        //Paint
        paint_enable = EditorGUILayout.Toggle("Paint", paint_enable);
        if (paint_enable && tool != Tools.PAINT)
        {
            tool = Tools.PAINT;
            UpdateToolCheck();
        }

        //Erase 
        erase_enable = EditorGUILayout.Toggle("Erase", erase_enable);
        if (erase_enable && tool != Tools.ERASE)
        {
            tool = Tools.ERASE;
            UpdateToolCheck();
        }

        //Rect
        rect_enable = EditorGUILayout.Toggle("Rect", rect_enable);
        if (rect_enable && tool != Tools.RECT)
        {
            tool = Tools.RECT;
            UpdateToolCheck();
        }

        //Dropper
        dropper_enable = EditorGUILayout.Toggle("Dropper", dropper_enable);
        if (dropper_enable && tool != Tools.DROPPER)
        {
            tool = Tools.DROPPER;
            UpdateToolCheck();
        }


        //Brushes ------------------------------------------------------------------------------------------------
        if (brush_obj != null)
            GUILayout.Label("Brushes [" + brush_obj.name + "]", EditorStyles.boldLabel);
        else
            GUILayout.Label("Brushes", EditorStyles.boldLabel);

        //Floor
        floor_prefab = (GameObject)EditorGUILayout.ObjectField("Floor: ", floor_prefab, typeof(GameObject), true);

        floor_height = EditorGUILayout.FloatField("Floor height: ", floor_height);

        if (floor_prefab == null)
        {
            EditorGUILayout.HelpBox("A floor prefab must be assigned.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Floor", EditorStyles.miniButtonLeft))
            {
                brush_obj = floor_prefab;
                brush = Brushes.FLOOR;
            }
        }

        //Wall
        wall_prefab = (GameObject)EditorGUILayout.ObjectField("Wall: ", wall_prefab, typeof(GameObject), true);

        wall_height = EditorGUILayout.FloatField("Wall height: ", wall_height);

        if (wall_prefab == null)
        {
            EditorGUILayout.HelpBox("A wall prefab must be assigned.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Wall", EditorStyles.miniButtonLeft))
            {
                brush_obj = wall_prefab;
                brush = Brushes.WALL;
            }
        }
    }

    /// <summary>
    /// Options of the level editor. Update of the features.
    /// </summary>
    private void Options()
    {
        GUILayout.Label("Options", EditorStyles.boldLabel);

        //Reset
        if (GUILayout.Button("Reset", EditorStyles.miniButtonMid))
        {
            Init();
        }

        //Optimize
        if (GUILayout.Button("Optimize", EditorStyles.miniButtonLeft))
        {
            OptimizeLevel();
        }

        //Take apart
        if (GUILayout.Button("Take apart", EditorStyles.miniButtonLeft))
        {
            TakeApart();
        }
    }

    /// <summary>
    /// Updates the checks of every tool when selected manually or by a shortcut.
    /// </summary>
    private void UpdateToolCheck()
    {
        //Update tools checkpoints
        //Set all false
        paint_enable = false;
        erase_enable = false;
        rect_enable = false;
        dropper_enable = false;

        //Only set the current tool to true
        switch (tool)
        {
            case Tools.PAINT:
                paint_enable = true;
                break;
            case Tools.ERASE:
                erase_enable = true;
                break;
            case Tools.RECT:
                rect_enable = true;
                break;
            case Tools.DROPPER:
                dropper_enable = true;
                break;
        }
    }

    /// <summary>
    /// Combines all the same objects in one.
    /// </summary>
    private void OptimizeLevel()
    {
        SavePositions();
        CombineMeshes(floor_container, "Floor");
        CombineMeshes(wall_container, "Wall");
    }
    /// <summary>
    /// Actual method that combines the meshes
    /// </summary>
    private void CombineMeshes(GameObject container, string name)
    {
        MeshFilter[] mesh_filters = container.GetComponentsInChildren<MeshFilter>();

        if (mesh_filters.Length == 0)
            return;

        CombineInstance[] combine = new CombineInstance[mesh_filters.Length];

        Material material = container.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        for (int i = 0; i < mesh_filters.Length; i++)
        {
            combine[i].mesh = mesh_filters[i].sharedMesh;
            combine[i].transform = mesh_filters[i].transform.localToWorldMatrix;
            DestroyImmediate(mesh_filters[i].gameObject);
        }

        GameObject result_combined = new GameObject();
        result_combined.name = name + "_Combined";
        result_combined.transform.SetParent(container.transform);

        MeshFilter result_filter = result_combined.AddComponent<MeshFilter>();
        MeshRenderer result_renderer = result_combined.AddComponent<MeshRenderer>();

        result_filter.sharedMesh = new Mesh();
        result_filter.sharedMesh.CombineMeshes(combine, true);
        result_renderer.material = material;
        result_combined.SetActive(true);
    }


    /// <summary>
    /// Breaks the combined object in multiple pieces. 
    /// </summary>
    private void TakeApart()
    {
        CreateIndividualObjects(floor_container, floor_positions, floor_prefab, "Floor");
        CreateIndividualObjects(wall_container, wall_positions, wall_prefab, "Wall");
    }
    /// <summary>
    /// Creates every object from the list of positions of every object.
    /// </summary>
    private void CreateIndividualObjects(GameObject container, List<Vector3> positions_list, GameObject obj, string obj_name)
    {
        //Erase childs
        Transform[] childs = container.transform.getChilds();
        for (int i = 0; i < childs.Length; i++)
        {
            DestroyImmediate(childs[i].gameObject);
        }
        //Create new objects
        foreach(Vector3 position in positions_list)
        {
            GameObject item = Instantiate(obj, position, new Quaternion()) as GameObject;
            item.name = obj_name;
            item.transform.SetParent(container.transform);
        }
    }

    /// <summary>
    /// Paints an object on the mouse position (if it's not already painted).
    /// </summary>
    private void CreateObject(Event e)
    {
        if (brush_obj != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            bool can_create = false; //The object is already created in that position?

            RaycastHit hit;

            //Calculate position to create
            float lenght = Vector3.Dot(-ray.origin, Vector3.up) / Vector3.Dot(ray.direction, Vector3.up);

            Vector3 position = ray.origin + (ray.direction * lenght);

            //Round X-Z
            position.x = Mathf.Round(position.x);
            position.z = Mathf.Round(position.z);


            if (Physics.Raycast(ray, out hit) == false)
            {
                can_create = true;
            }
            else
            {
                if (hit.transform.tag != brush_obj.tag)
                {
                    can_create = true;
                }
            }

            if (can_create) //Create the object
            {
                GameObject obj = (GameObject)Instantiate(brush_obj);

                switch (brush)
                {
                    case Brushes.FLOOR:
                        obj.transform.SetParent(floor_container.transform);
                        obj.name = "Floor";
                        position.y = floor_height;
                        break;
                    case Brushes.WALL:
                        obj.transform.SetParent(wall_container.transform);
                        obj.name = "Wall";
                        position.y = wall_height;
                        break;
                    case Brushes.NONE:
                        Debug.Log("LEVEL EDITOR ERROR: no brush has been assigned. The object cannot be placed.");
                        break;
                }

                obj.transform.position = position;
            }
        }
    }

    /// <summary>
    /// Erases the object under the mouse.
    /// </summary>
    private void EraseObject(Event e)
    {
        if (brush_obj == null)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) != false)
        {
            if (hit.transform.tag == brush_obj.tag)
            {
                DestroyImmediate(hit.transform.gameObject);
            }
        }
    }

    /// <summary>
    /// Sets the brush object to the object under the mouse.
    /// </summary>
    private void DropperTool(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) != false)
        {
            SearchBrushByTag(hit.transform.tag);
            Repaint();
        }
    }

    /// <summary>
    /// Searches a Brush by his tag and sets the current brush.
    /// </summary>
    private void SearchBrushByTag(string tag)
    {
        brush_obj = null;
        brush = Brushes.NONE;

        if (floor_prefab.tag == tag)
        {
            brush_obj = floor_prefab;
            brush = Brushes.FLOOR;
        }


        if (wall_prefab.tag == tag)
        {
            brush_obj = wall_prefab;
            brush = Brushes.WALL;
        }
    }

    private void RectTool(Event e)
    {
        if (e.type == EventType.mouseDown && e.button == 0 && e.alt == false)
        {
            //Save down position as reference
        }

        if(e.type == EventType.mouseUp && e.button == 0 && e.alt == false)
        {

        }

        //Save up position as last position
        //Calculate up-left and down-right
        //Create object and check if the position is already created
    }
}
