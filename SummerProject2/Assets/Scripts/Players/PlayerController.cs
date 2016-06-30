using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Selection
    GameObject selection_circle;
    [HideInInspector]
    public bool is_selected;

    void Awake()
    {
        selection_circle = transform.GetChild(0).gameObject;
    }
	// Use this for initialization
	void Start ()
    {
        is_selected = false;
        selection_circle.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {

        //Show/Hide selection circle
        if(is_selected)
        {
            selection_circle.SetActive(true);
        }
        else
        {
            selection_circle.SetActive(false);
        }
	}
}
