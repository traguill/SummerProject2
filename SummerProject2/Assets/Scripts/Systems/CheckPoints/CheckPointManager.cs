using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPointManager : MonoBehaviour {


    private static bool created = false;

    public List<CheckPoint> checkpoints;

    int checkpoint_id = 0; //Current progress of the checkpoint

    Vector3 spawn_position = new Vector3();

    void Awake()
    {
        if(!created)
        {
            DontDestroyOnLoad(transform.gameObject);
            created = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        checkpoint_id = 0;
    }

    /// <summary>
    /// Called when the level is loaded again or for the first time
    /// </summary>
    public void InitializeCheckpoints(Transform start_position)
    {

        //Set manager reference
        foreach(CheckPoint point in checkpoints)
        {
            point.InitializeManager(this);
        }

        //Activate the next checkpoint
        if(checkpoint_id < checkpoints.Count)
            checkpoints[checkpoint_id].active = true; 

        //First checkpoint passed
        if(checkpoint_id != 0)
        {
            spawn_position = checkpoints[checkpoint_id - 1].GetSpawnPosition();
            start_position.position = spawn_position;
        }
        
    }

    public void NextCheckPoint()
    {
        checkpoints[checkpoint_id].active = false;
        checkpoint_id += 1;
        if (checkpoint_id < checkpoints.Count)
        {
            checkpoints[checkpoint_id].active = true;
        }
    }

}
