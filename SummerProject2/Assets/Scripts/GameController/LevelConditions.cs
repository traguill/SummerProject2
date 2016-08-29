using UnityEngine;
using System.Collections;

public class LevelConditions : MonoBehaviour
{
    private GameObject[] players;
    private Transform start_point;
    private ScreenFader screen_fader;
    private float radius;
    public static int players_at_end_level;

    // Call before any Start function
    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag(Tags.player);
        screen_fader = GameObject.FindGameObjectWithTag(Tags.screen_fader).GetComponent<ScreenFader>();

        start_point = GameObject.Find("Start_point").transform;   // I don't know any other efficient way!
        radius = 3.5f;
        players_at_end_level = 0;
    }

    // Use this for initialization
    void Start()
    {
        // The different players is distributed equidistantly on a circle of radius "radius". The script automatically take into account
        // the number of players to put along the circle.
        float angle = 0.0f;
        float incr_angle = ((2 * Mathf.PI) / players.Length);

        foreach (GameObject p in players)
        {
            p.transform.position = new Vector3(radius * Mathf.Cos(angle) + start_point.position.x, 0, radius * Mathf.Sin(angle) + start_point.position.z);
            angle += incr_angle;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (CheckFinishingLevel())
        {
            screen_fader.EndScene(0);
        }
    }

    /// <summary>
    /// checkFinishingLevel() checks if the number of players on the trigger are the total number of players.
    /// </summary>
    /// <returns> True if all the players are on the finish point. False otherwise </returns>
    bool CheckFinishingLevel()
    {
        return players_at_end_level == players.Length;
    }   
}
