using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelEditor : EditorWindow
{
    GameObject brush_obj = null;

    //Prefabs
    GameObject floor_prefab = null;
    GameObject wall_prefab = null;

    //Options
    bool paint_enable = false;
    bool erase_enable = false;

    [MenuItem("Window/Level Editor")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelEditor));
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate = OnSceneGUI;
    }


    void OnGUI()
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
        if(brush_obj != null)
            GUILayout.Label("Brushes ["+brush_obj.name+"]", EditorStyles.boldLabel);
        else
            GUILayout.Label("Brushes", EditorStyles.boldLabel);

        //Floor
        floor_prefab = (GameObject)EditorGUILayout.ObjectField("Floor: ", floor_prefab, typeof(GameObject), true);

        if (floor_prefab == null)
        {
            EditorGUILayout.HelpBox("A floor prefab must be assigned.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Floor", EditorStyles.miniButtonLeft))
            {
                brush_obj = floor_prefab;
            }
        }

        //Wall
        wall_prefab = (GameObject)EditorGUILayout.ObjectField("Wall: ", wall_prefab, typeof(GameObject), true);

        if (wall_prefab == null)
        {
            EditorGUILayout.HelpBox("A wall prefab must be assigned.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Wall", EditorStyles.miniButtonLeft))
            {
                brush_obj = wall_prefab;
            }
        }
   
    }

    void OnSceneGUI(SceneView sceneView)
    {   
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

            position.x = Mathf.Round(position.x);
            position.y = 0.5f;
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
