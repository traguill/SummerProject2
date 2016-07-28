using UnityEngine;
using System.Collections;

public class LevelConditions : MonoBehaviour
{
    private GameObject[] players;
    private Transform start_point, finish_point;
    private ScreenFader screen_fader;
    private float radius;
    private uint num_players_reaching_end_level;

    // Call before any Start function
    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag(Tags.player);
        screen_fader = GameObject.FindGameObjectWithTag(Tags.screen_fader).GetComponent<ScreenFader>();

        start_point = transform.GetChild(0);
        finish_point = transform.GetChild(1);
        radius = 3.5f;
        num_players_reaching_end_level = 0;
    }

    // Use this for initialization
    void Start()
    {
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
        if (checkFinishingLevel())
        {
            screen_fader.EndScene(0);
        }
    }

    bool checkFinishingLevel()
    {
        return num_players_reaching_end_level == players.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.player))
            num_players_reaching_end_level++;

        Debug.Log("Somebody has entered: " + num_players_reaching_end_level);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.player))
            num_players_reaching_end_level--;

        Debug.Log("Somebody has leaved: " + num_players_reaching_end_level);
    }

    
}
