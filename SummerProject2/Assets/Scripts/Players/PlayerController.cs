using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Selection
    GameObject selection_circle;
    [HideInInspector]
    public bool is_selected;

    private NavMeshAgent agent;

    void Awake()
    {
        selection_circle = transform.GetChild(0).gameObject;
    }
	// Use this for initialization
	void Start ()
    {
        is_selected = false;
        selection_circle.SetActive(false);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update ()
    {

        //Show/Hide selection circle
        if(is_selected)
        {
            selection_circle.SetActive(true);
            if(Input.GetMouseButton(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    agent.destination = hit.point;
                }
            }            
        }
        else
        {
            selection_circle.SetActive(false);
        }
	}
}
