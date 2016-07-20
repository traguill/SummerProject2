using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;

[Serializable]
public class OnEvent : UnityEvent{}

//Radial button template

public class RadialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public OnEvent function;
    public Image background;
    public Image icon;
    public string title;
    public RadialMenu main_menu;

    Color default_color; //Change color on hover button (in the future change this for better animations)

    public void OnPointerEnter(PointerEventData eventData)
    {
        default_color = background.color; //Update background color
        background.color = Color.white;

        main_menu.selected = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        main_menu.selected = null;
        background.color = default_color; //Update background color
    }
}
