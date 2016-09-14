using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

    public bool use_barion = true;
    public bool use_cosmo = true;
    public bool use_nyx = true;

    [HideInInspector]public bool active = false;

    bool barion_activated = false;
    bool cosmo_activated = false;
    bool nyx_activated = false;

    bool completed = false;

    CheckPointManager manager = null;

    Vector3 spawn_position;

    public void InitializeManager(CheckPointManager _manager)
    {
        manager = _manager;
    }

    void Awake()
    {
        spawn_position = transform.GetChild(0).position;
    }

    void Update()
    {
        if(!completed && active)
        {
            if((use_barion && barion_activated) && (use_cosmo && cosmo_activated) && (use_nyx && nyx_activated))
            {
                completed = true;
                if(manager)
                {
                    manager.NextCheckPoint(); //Go to next checkpoint
                }
            }
        }
    }


    void OnTriggerEnter(Collider col)
    {
        if (!active)
            return;

        if(col.tag == Tags.player)
        {
            if (col.name == Objects.barion)
                barion_activated = true;

            if (col.name == Objects.cosmo)
                cosmo_activated = true;

            if (col.name == Objects.nyx)
                nyx_activated = true;
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return spawn_position;
    }
}
