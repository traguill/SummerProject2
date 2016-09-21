using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;


public class LevelEditor : EditorWindow
{
    Brush brush_obj = null;

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
    bool editor_enable = false;
    bool paint_enable = false;
    bool erase_enable = false;
    bool rect_enable = false;
    bool dropper_enable = false;
    

    List<Brush> brushes_2 = new List<Brush>();
    List<GameObject> brush_containers = new List<GameObject>();

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
        if (level == null)
        {
            level = new GameObject();
            level.name = "Level";
        }
        //Load Settings
        GameObject settings = GameObject.Find("LevelEditorSettings");

        //Load Brushes
        GameObject brushes_container = settings.transform.FindChild("Brushes").gameObject;

        brushes_2.Clear();

        int id = 0;
        foreach(Transform brush_object in brushes_container.transform.getChilds())
        {
            Brush brush = brush_object.GetComponent<Brush>();
            brush.id = id;
            brushes_2.Add(brush);
            ++id;
        }

       
        foreach(Brush brush_object in brushes_2)
        {
            string container_name = brush_object.folder_name;
            GameObject container = level.transform.FindChild(container_name).gameObject;
            if(container == null) //Create folder if doesn't exist
            {
                container = new GameObject();
                container.transform.SetParent(level.transform);
                container.name = container_name;               
            }
            brush_containers.Add(container);
        }

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
        int id = 0;
        foreach(Brush brush_obj in brushes_2)
        {
            Transform[] childs = brush_containers[id].transform.getChilds();
            for (int i = 0; i < childs.Length; i++)
                brush_obj.obj_positions.Add(childs[i].position);

            ++id;
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

        foreach(Brush brush_item in brushes_2)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(brush_item.object_name);
            if(GUILayout.Button("Select"))
            {
                brush_obj = brush_item;
            }
            if (GUILayout.Button("Edit"))
            {
                Debug.Log(brush_item.object_name + " Edit");
            }
            GUILayout.EndHorizontal();
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
        foreach(GameObject brush in brush_containers)
        {
            CombineMeshes(brush, "CombinedMesh"); //TODO set a name
        }
        
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
        int id = 0;
        foreach(Brush brush in brushes_2)
        {
            CreateIndividualObjects(brush_containers[id], brush.obj_positions, brush.obj, brush.object_name);
            ++id;
        }
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
                GameObject obj = (GameObject)Instantiate(brush_obj.obj);

                obj.name = brush_obj.object_name;
                position.y = brush_obj.height;
                obj.transform.SetParent(brush_containers[brush_obj.id].transform);

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
            if (hit.transform.tag == brush_obj.obj.tag)
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

        foreach(Brush brush in brushes_2)
        {
            if(brush.obj.tag == tag)
            {
                brush_obj = brush;
                return;
            }
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
