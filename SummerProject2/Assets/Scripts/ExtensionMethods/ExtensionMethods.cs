using UnityEngine;
using System.Collections;

public static class ExtensionMethods {

    /// <summary>
    /// getChilds: Own script to get all the direct childs
    /// Complaints to CRZ
    /// </summary>

    public static Transform[] getChilds (this Transform trans)
    {
        Transform[] ret;
        int num_of_childs = trans.childCount;
        if (num_of_childs > 0)
        {
            ret = new Transform[num_of_childs];
            for(int i = 0; i < num_of_childs; ++i)
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
}
