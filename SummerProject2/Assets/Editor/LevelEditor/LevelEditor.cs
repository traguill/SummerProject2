using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelEditor : EditorWindow
{
    GameObject brush_obj = null;

    //Prefabs (brush)
    GameObject floor_prefab = null;
    GameObject wall_prefab = null;

    //Creation height
    float floor_height = 0.05f;
    float wall_height = 0.5f;

    //Brushes
    enum Brushes
    {
        NONE,
        WALL,
        FLOOR
    }

    Brushes brush = Brushes.NONE;

    //Options
    bool paint_enable = false;
    bool erase_enable = false;
    bool editor_enable = false;

    //Containers
    GameObject wall_container;
    GameObject floor_container;

    //Saved data
    Vector3[] floor_positions = new Vector3[0];
    Vector3[] wall_positions = new Vector3[0];

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

    private void SavePositions()
    {
        //Save objects position
        floor_positions = new Vector3[floor_container.transform.childCount];
        Transform[] childs_floor = floor_container.transform.getChilds();
        for (int i = 0; i < childs_floor.Length; i++)
        {
            floor_positions[i] = childs_floor[i].position;
        }

        wall_positions = new Vector3[wall_container.transform.childCount];
        Transform[] childs_wall = wall_container.transform.getChilds();
        for (int i = 0; i < childs_wall.Length; i++)
        {
            wall_positions[i] = childs_wall[i].position;
        }
    }


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

    private void ToolOptions()
    {
        //Tools-------------------------------------------------------------------------------------------------
        GUILayout.Label("Tools", EditorStyles.boldLabel);

        //Paint
        paint_enable = EditorGUILayout.Toggle("Paint", paint_enable);

        //Conditions
        if (paint_enable == true)
            erase_enable = false;

        //Erase 
        erase_enable = EditorGUILayout.Toggle("Erase", erase_enable);
        //Conditions
        if (erase_enable == true)
            paint_enable = false;


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

    private void Options()
    {
        GUILayout.Label("Options", EditorStyles.boldLabel);

        //Reset
        if (GUILayout.Button("Reset", EditorStyles.miniButtonMid)) 
        {
            Init();
        }

        //Optimize
        if(GUILayout.Button("Optimize", EditorStyles.miniButtonLeft))
        {
            OptimizeLevel();
        }

        //Take apart
        if(GUILayout.Button("Take apart", EditorStyles.miniButtonLeft))
        {
            TakeApart();
        }
    }

    private void OptimizeLevel()
    {
        SavePositions();
        CombineMeshes(floor_container, "Floor");
        CombineMeshes(wall_container, "Wall");     
    }

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

    private void TakeApart()
    {
        CreateIndividualObjects(floor_container, floor_positions, floor_prefab, "Floor");
        CreateIndividualObjects(wall_container, wall_positions, wall_prefab, "Wall");
    }

    private void CreateIndividualObjects(GameObject container, Vector3[] positions_list, GameObject obj, string obj_name)
    {
        //Erase childs
        Transform[] childs = container.transform.getChilds();
        for(int i = 0; i < childs.Length; i++)
        {
            DestroyImmediate(childs[i].gameObject);
        }
        Debug.Log(positions_list.Length);
        //Create new objects
        for (int i = 0; i < positions_list.Length; i++)
        {
            GameObject item = Instantiate(obj, positions_list[i], new Quaternion()) as GameObject;
            item.name = obj_name;
            item.transform.SetParent(container.transform);
        }

    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!editor_enable)
            return;

        //Shortcuts
        Event e = Event.current;

        if (e.keyCode == KeyCode.B && e.type == EventType.keyDown)
        {
            paint_enable = true;
            erase_enable = false;
            Repaint();
        }

        if (e.keyCode == KeyCode.E && e.type == EventType.keyDown)
        {
            erase_enable = true;
            paint_enable = false;
            Repaint();
        }

        if(paint_enable || erase_enable)
        {
            HandleUtility.AddDefaultControl(0); //Disable engine input

            if ((e.type == EventType.mouseDown || e.type == EventType.mouseDrag) && e.button == 0 && e.alt == false)
            {    
                if(paint_enable)
                    CreateObject(e);

                if (erase_enable)
                    EraseObject(e);
            }
        }    
    }

    private void CreateObject(Event e)
    {
        if(brush_obj != null)
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


            if(Physics.Raycast(ray, out hit) == false)
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

            if(can_create) //Create the object
            {
                GameObject obj = (GameObject)Instantiate(brush_obj);
                

                switch(brush)
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
}
