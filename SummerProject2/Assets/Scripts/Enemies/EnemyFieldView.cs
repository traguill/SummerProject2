using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFieldView : MonoBehaviour
{

    public float view_radius;
    [Range(0, 360)]
    public float view_angle;

    public LayerMask target_mask;
    public LayerMask obstacle_mask;

    private AlarmSystem alarm_system;
    private ScreenFader screen_fader;

    [HideInInspector]
    public List<Transform> visible_targets = new List<Transform>();


    // Awake
    void Awake()
    {
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        screen_fader = GameObject.FindGameObjectWithTag(Tags.screen_fader).GetComponent<ScreenFader>();
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if(FindVisibleTargets())
            {
                CheckVisibleTargets();
            }            
        }
    }

    /// <summary>
    /// FindVisibleTargets() looks if any player and enemy is on the enemy vision cone, each XX seconds, according to FindTargetsWithDelay.
    /// Visible_targets includes every player and/or enemy positions.
    /// </summary>
    /// <return> true whether some visible objects has been spotted. False otherwise. </return>
    bool FindVisibleTargets()
    {
        visible_targets.Clear();
        Collider[] targets_in_view_radius = Physics.OverlapSphere(transform.position, view_radius, target_mask);

        Debug.Log("Number of colliders: " + targets_in_view_radius.Length);

        for (int i = 0; i < targets_in_view_radius.Length; i++)
        {
            Transform target = targets_in_view_radius[i].transform;
            Vector3 direction = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, direction) < view_angle / 2)
            {
                float distance_to_target = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, direction, distance_to_target, obstacle_mask))
                {
                    visible_targets.Add(target);
                }
            }
        }
        
        return visible_targets.Count > 0;
    }

    /// <summary>
    /// CheckVisibleTargets() checks every Transform gameObject and checks its Tag. If player is found, the level resets. 
    /// If enemy corpse is found, alert mode is activated
    /// </summary>
    void CheckVisibleTargets()
    {
        foreach(Transform t in visible_targets)
        {
            // If enemy corpse is found, alert mode is activated
            if (t.tag.Equals(Tags.corpse))
                alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);

            // If player is found, the level resets.
            if (t.tag.Equals(Tags.player))
                screen_fader.EndScene(0);
        }        
    }

    public Vector3 DirectionFromAngle(float angle, bool angle_is_global)
    {
        if (!angle_is_global)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

}
