using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HUDAbilityDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  {

    //Timer to show the description
    bool start_timer = false;
    float timer = 0.0f;
    float show_time = 1.0f;

    //Description Info TODO: load this from an external text file
    public string name;
    public string key;
    public string cooldown;
    [TextArea]
    public string description;

    HUD_Controller hud_controller;

    void Awake()
    {
        hud_controller = GameObject.Find(Objects.HUD).GetComponent<HUD_Controller>();
    }

    void Update()
    {
        if(start_timer)
        {
            timer += Time.deltaTime;
            if(timer >= show_time)
            {
                hud_controller.ShowAbilityDescription(name, key, cooldown, description);
                start_timer = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        start_timer = true;
        timer = 0.0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        timer = 0.0f;
        start_timer = false;

        hud_controller.HideAbilityDescription();
    }
}
