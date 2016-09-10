using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityDescription : MonoBehaviour 
{

    public  Text name;
    public  Text key;
    public  Text cooldown;
    public  Text description;

	// Use this for initialization
	void Start () 
    {
        gameObject.SetActive(false); //Start hide
	}


    public void SetName(string _name)
    {
        name.text = "<color=white>" + _name + "</color>";
    }

    public void SetKey(string _key)
    {
        key.text = "<color=orange>[" + _key + "]</color>";
    }

    public void SetCooldown(string _cooldown)
    {
        cooldown.text = "<color=white>" + _cooldown + "sec cooldown" + "</color>";
    }

    public void SetDescription(string _description)
    {
        description.text = "<color=lightblue>" + _description + "</color>";
    }
}
