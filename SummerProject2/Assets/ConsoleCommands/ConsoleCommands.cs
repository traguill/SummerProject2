using UnityEngine;
using System.Collections;

public class ConsoleCommands : MonoBehaviour {

    private ConsoleCommandsRepository console;

    //Reference to other scripts
    EnemyManager enemy_manager;

    void Awake()
    {
        enemy_manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }

	// Use this for initialization
	void Start () 
    {
        console = ConsoleCommandsRepository.Instance;

        AddCommands();
	}

    private void AddCommands()
    {
        console.RegisterCommand("fow", FogOfWar);
    }


    private string FogOfWar(params string[] args)
    {
        bool enabled = bool.Parse(args[0]);

        enemy_manager.fow_disabled = !enabled;

        string ret;

        if (enabled)
            ret = "Fog of war is now enabled";
        else
            ret = "Fog of war is now disabled";
        return ret;
    }
}
