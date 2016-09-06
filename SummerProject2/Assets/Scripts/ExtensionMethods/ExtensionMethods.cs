using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    /// <summary>
    /// getChilds: Own script to get all the direct childs.
    /// Complaints to CRZ!
    /// </summary>
    public static Transform[] getChilds(this Transform trans)
    {
        Transform[] ret;
        int num_of_childs = trans.childCount;
        if (num_of_childs > 0)
        {
            ret = new Transform[num_of_childs];
            for (int i = 0; i < num_of_childs; ++i)
            {
                ret[i] = trans.GetChild(i);
            }
        }
        else
        {
            ret = new Transform[1];
            ret[0] = null;
        }

        return ret;
    }

    public static void SetPosition(this Transform trans, Vector3 new_pos)
    {
        trans.position = new_pos;
    }

    public static void UpdatePosition(this Transform trans, float x, float y, float z)
    {
        trans.position += new Vector3(x,y,z);
    }

    /// <summary>
    /// Determines if a vector is close to another vector within a delta threshold
    /// </summary>
    /// <param name="our_vector"></param>
    /// <param name="vec_to_compare"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static bool IsCloseTo(this Vector3 our_vector, Vector3 vec_to_compare, float delta)
    {
        return (Mathf.Abs(our_vector.x - vec_to_compare.x) < delta && 
                Mathf.Abs(our_vector.y - vec_to_compare.y) < delta && 
                Mathf.Abs(our_vector.z - vec_to_compare.z) < delta);
    }

}
