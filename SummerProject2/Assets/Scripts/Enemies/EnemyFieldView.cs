using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ALARM_STATE : byte
{
    ALARM_ON,
    ALARM_OFF
};

public class EnemyFieldView : MonoBehaviour {

    public float view_radius;
    [Range(0, 360)]
    public float view_angle;

    public LayerMask target_mask;
    public LayerMask obstacle_mask;    

    public static ALARM_STATE alarm_state;                

    [HideInInspector]
    public List<Transform> visible_targets = new List<Transform>();

    // Use this for initialization
    void Start ()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);
        alarm_state = ALARM_STATE.ALARM_OFF;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    /// <summary>
    /// FindVisibleTargets() looks if any player is on the enemy vision cone, each XX seconds, according to FindTargetsWithDelay.
    /// Visible_targets includes every player position.
    /// </summary>
    void FindVisibleTargets()
    {
        visible_targets.Clear();
        Collider[] targets_in_view_radius = Physics.OverlapSphere(transform.position, view_radius, target_mask);

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
                    alarm_state = ALARM_STATE.ALARM_ON;
                }
            }
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

    public ALARM_STATE getAlarmState()
    {
        return alarm_state;
    }
}
