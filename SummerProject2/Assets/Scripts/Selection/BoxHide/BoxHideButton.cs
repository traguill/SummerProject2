using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;

[Serializable]
public class OnEventSimple : UnityEvent {} //This should go in a separate script.

public class BoxHideButton : MonoBehaviour,  IPointerClickHandler{

    public OnEventSimple function;
    public Image icon;
    public BoxHideMenu main_menu;
    public string name;

    public void OnPointerClick(PointerEventData eventData)
    {
        function.Invoke();
    }
  
}
