using UnityEngine;
using System.Collections;

public class AnimatedTiledTexture : MonoBehaviour
{
    //Note: X goes up and Y goes down

    public int tiles_x;
    public int tiles_y;

    public float duration;

    public bool use_projector = false;

    private float min_step_x;
    private float max_step_x;
    private float min_step_y;
    private float max_step_y;

    private float step_x;
    private float step_y;

    private float offset_x;
    private float offset_y;

    private float timer;
    private float time_step;

    Material mat;

    string main_tex;

    void Awake()
    {
        if (use_projector == false)
        {
            mat = GetComponent<Renderer>().material;
            main_tex = "_MainTex";
        }
        else
        {
            mat = GetComponent<Projector>().material;
            main_tex = "_ShadowTex";
        }
            
    }

    void Start()
    {
        min_step_x = 0;
        max_step_y = 0;

        max_step_x = (1.0f / tiles_x) * (tiles_x - 1);
        min_step_y = (1.0f / tiles_y) * (tiles_y - 1);

        step_x = 1.0f / tiles_x;
        step_y = 1.0f / tiles_y;

        //Initialize
        offset_x = min_step_x;
        offset_y = min_step_y;

        time_step = duration / (tiles_x * tiles_y);
        timer = 0;

        mat.SetTextureScale(main_tex, new Vector2(step_x, step_y));
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= time_step)
        {
            //Update frame
            offset_x += step_x;

            if (offset_x > max_step_x)
            {
                offset_x = min_step_x;
                offset_y -= step_y;

                if (offset_y < max_step_y)
                {
                    //Restart
                    offset_y = min_step_y;
                }
            }

            //Set the offset
            mat.SetTextureOffset(main_tex, new Vector2(offset_x, offset_y));

            timer = 0.0f;
        }
       
    }
}