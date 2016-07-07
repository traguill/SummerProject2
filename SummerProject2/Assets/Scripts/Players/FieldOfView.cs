using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour 
{

    public float view_radius;
    [Range (0,360)]
    public float view_angle;

    public LayerMask target_mask;
    public LayerMask obstacle_mask;

    [HideInInspector]
    public List<Transform> visible_targets = new List<Transform>();

    public float mesh_resolution;
    public int edge_resolve_iterations;
    public float edge_dst_threshold;

    public MeshFilter view_mesh_filter;
    Mesh view_mesh;

    void Start()
    {
        view_mesh = new Mesh();
        view_mesh.name = "View Mesh";
        view_mesh_filter.mesh = view_mesh;
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    void Update()
    {
        DrawFieldOfView();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visible_targets.Clear();
        Collider[] targets_in_view_radius = Physics.OverlapSphere(transform.position, view_radius, target_mask);

        for(int i = 0; i < targets_in_view_radius.Length; i++)
        {
            Transform target = targets_in_view_radius[i].transform;
            Vector3 direction = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, direction) < view_angle / 2)
            {
                float distance_to_target = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, direction, distance_to_target, obstacle_mask))
                {
                    visible_targets.Add(target);
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int step_count = Mathf.RoundToInt(view_angle * mesh_resolution);
        float step_angle_size = view_angle / step_count;

        List<Vector3> view_points = new List<Vector3>();

        ViewCastInfo prev_viewcast = new ViewCastInfo();
        //Create a raycast for the mesh resolution
        for(int i = 0; i <= step_count; i++)
        {
            float angle = transform.eulerAngles.y - view_angle / 2 + step_angle_size * i;
            ViewCastInfo new_view_cast = ViewCast(angle);

            if(i > 0)
            {
                bool edge_dst_threshold_exceeded = Mathf.Abs(prev_viewcast.dst - new_view_cast.dst) > edge_dst_threshold;
                if(prev_viewcast.hit != new_view_cast.hit || (prev_viewcast.hit && new_view_cast.hit && edge_dst_threshold_exceeded))
                {
                    EdgeInfo edge = FindEdge(prev_viewcast, new_view_cast);
                    if(edge.pointA != Vector3.zero)
                    {
                        view_points.Add(edge.pointA);
                    }

                    if (edge.pointB != Vector3.zero)
                    {
                        view_points.Add(edge.pointB);
                    }
                }
            }

            view_points.Add(new_view_cast.point);

            prev_viewcast = new_view_cast;
        }

        //Calculate the vertices and triangles to create the mesh
        int vertex_count = view_points.Count + 1;
        Vector3[] vertices = new Vector3[vertex_count];
        int[] triangles = new int[(vertex_count - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertex_count -1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(view_points[i]);

            if(i < vertex_count - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            
        }
        view_mesh.Clear();
        view_mesh.vertices = vertices;
        view_mesh.triangles = triangles;
        view_mesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo min_viewcast, ViewCastInfo max_viewcast)
    {
        float min_angle = min_viewcast.angle;
        float max_angle = max_viewcast.angle;

        Vector3 min_point = Vector3.zero;
        Vector3 max_point = Vector3.zero;

        for (int i = 0; i < edge_resolve_iterations; i++)
        {
            float angle = (min_angle + max_angle) / 2;
            ViewCastInfo new_viewcast = ViewCast(angle);

            bool edge_dst_threshold_exceeded = Mathf.Abs(min_viewcast.dst - new_viewcast.dst) > edge_dst_threshold;

            if(new_viewcast.hit == min_viewcast.hit && !edge_dst_threshold_exceeded)
            {
                min_angle = angle;
                min_point = new_viewcast.point;
            }
            else
            {
                max_angle = angle;
                max_point = new_viewcast.point;
            }
        }

        return new EdgeInfo(min_point, max_point);
    }

    ViewCastInfo ViewCast(float global_angle)
    {
        Vector3 direction = DirectionFromAngle(global_angle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, direction, out hit, view_radius, obstacle_mask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, global_angle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * view_radius, view_radius, global_angle);
        }
    }

    /// <summary>
    /// Returns the direction from a given angle (in degrees).
    /// </summary>
	public Vector3 DirectionFromAngle(float angle, bool angle_is_global)
    {
        if(!angle_is_global)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA,Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
