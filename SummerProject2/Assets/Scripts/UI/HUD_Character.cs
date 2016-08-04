using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class HUD_Character : MonoBehaviour, IPointerClickHandler
{
    UnitSelection selection_system;

    public GameObject character;

    public GameObject selected_sprite;
    public GameObject unselected_sprite;

    [HideInInspector] public bool selected = false;

    void Awake()
    {
        selection_system = GameObject.Find("SelectionSystem").GetComponent<UnitSelection>();
    }

    void Update()
    {
        bool is_selected = selection_system.IsPlayerSelected(character);

        if(is_selected != selected) //Selection has changed (update sprite)
        {
            if(is_selected == false)
            {
                unselected_sprite.SetActive(true);
                selected_sprite.SetActive(false);
                selected = false;
            }
            else
            {
                selected_sprite.SetActive(true);
                unselected_sprite.SetActive(false);
                selected = true;
            }
        }
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
