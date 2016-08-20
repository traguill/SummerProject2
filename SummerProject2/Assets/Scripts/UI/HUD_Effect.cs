using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD_Effect : MonoBehaviour 
{
    HUD_Controller hud_controller; //Instance to hud manager

    float life; //Duration of the clock
    float timer = 0.0f;
    Image clock;

    bool start_clock = false; //Says when the clock has to start

    int id; //Number of effect to identify it at hud controller to erase it later.

    Enums.Characters character; 

    void Awake()
    {
        clock = transform.GetChild(0).GetComponent<Image>();
    }


    // Update is called once per frame
    void Update()
    {
        if (start_clock == false)
            return;

        timer += Time.deltaTime;

        if (timer > life)
        {
            hud_controller.EffectFinished(character, id);
            Destroy(gameObject); //END
            return;
        }

        float value = timer / life;
        clock.fillAmount = 1 - value;
    }

    public void Initialize(Sprite image, Vector3 position, float time, HUD_Controller inst, Enums.Characters player, int id_num)
    {
        Image img = GetComponent<Image>();

        img.sprite = image;
        img.rectTransform.localPosition = position;
        life = time;
        hud_controller = inst;
        character = player;
        id = id_num;
        start_clock = true;

    }
}
