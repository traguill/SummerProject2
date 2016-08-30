using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFieldView : MonoBehaviour
{
    public float view_radius;
    [Range(0, 360)] public float view_angle;

    public LayerMask target_mask;
    public LayerMask obstacle_mask;

    private AlarmSystem alarm_system;
    private ScreenFader screen_fader;
    private EnemyManager enemy_manager;
    private CameraMove main_camera;
    private LastSpottedPosition last_spotted_position;

    public Queue<GameObject> pending_identification_elements;

    private bool player_found;

    [HideInInspector] public List<Transform> visible_targets = new List<Transform>();

    // Awake
    void Awake()
    {
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();        
        screen_fader = GameObject.FindGameObjectWithTag(Tags.screen_fader).GetComponent<ScreenFader>();
        enemy_manager = GetComponentInParent<EnemyManager>();
        main_camera = GameObject.FindGameObjectWithTag(Tags.main_camera).GetComponent<CameraMove>();
        last_spotted_position = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<LastSpottedPosition>();
    }

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.25f);
        player_found = false;
        pending_identification_elements = new Queue<GameObject>();
    }

     IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (FindVisibleTargets())
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
    public bool FindVisibleTargets()
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
                }
            }
        }

        return visible_targets.Count > 0;
    }

    /// <summary>
    /// CheckVisibleTargets() checks every Transform gameObject returned by FindVisibleTargets() and checks its Tag.
    /// If player is found, the level resets. If an enemy corpse is found, alert mode is activated.
    /// </summary>
    public void CheckVisibleTargets()
    {
        if (enemy_manager.god_mode == true) //Don't check visible characters if god mode is activated. 
            return;

        foreach(Transform t in visible_targets)
        {
            // Response for players!
            if (t.tag.Equals(Tags.player))
            {
                if (t.GetComponent<Invisible>().IsInvisible())
                    continue;
                if(!player_found)
                {
                    player_found = true;
                    main_camera.MoveCameraTo(transform.position);
                    screen_fader.EndScene(0);

                    // Debug part
                    //---------------------------
                    // Tint yellow the enemy who discovered a character
                    GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 0.0f, 0.0f));
                    //---------------------------
                }
            }         

            if(!enemy_manager.IsElementAlreadyIdentify(t.gameObject))
            {
                // Response: If something is found, the enemy will move
                // to a near position to cleary identify that element
                if ((t.tag.Equals(Tags.corpse) || t.tag.Equals(Tags.portal)) && !IsElementIdentificationPending(t.gameObject) )
                {
                    pending_identification_elements.Enqueue(t.gameObject);
                    RhandorController rhandor = GetComponent<RhandorController>();
                    last_spotted_position.LastPosition = t.transform.position;                
                    rhandor.ChangeStateTo(rhandor.spotted_state);                     
                }                
            }
        }        
    }

    private bool IsElementIdentificationPending(GameObject element_to_check)
    {
        return pending_identification_elements.Contains(element_to_check);
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
