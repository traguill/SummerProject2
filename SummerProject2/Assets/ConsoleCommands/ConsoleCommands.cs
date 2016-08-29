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
        console.RegisterCommand("time", TimeScale);
        console.RegisterCommand("god_mode", GodMode);
    }


    /// <summary>
    /// Shows or hides the enemies inside the fog of war
    /// </summary>
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

    /// <summary>
    /// Sets the time scale to a new value (1 - real time / 0 - no time)
    /// </summary>
    private string TimeScale(params string[] args)
    {
        float value = float.Parse(args[0]);

        Time.timeScale = value;

        return "Time now is " + value * 100 + "%";
    }


    private string GodMode(params string[] args)
    {
        bool enabled = bool.Parse(args[0]);

        enemy_manager.god_mode = enabled;

        return (enabled) ? "God Mode is enabled." : "God mode is disabled";
    }
}
