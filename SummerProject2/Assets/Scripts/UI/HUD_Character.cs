using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class HUD_Character : MonoBehaviour, IPointerClickHandler
{
    UnitSelection selection_system;

    public GameObject character;

    void Awake()
    {
        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int click_num = eventData.clickCount;

        if (click_num == 2)
        {
            print("Move camera to: "+character.name);

            return;
        }

        if (click_num == 1)
        {
            selection_system.AutoSelectPlayer(character);
        }

       
    }

}
