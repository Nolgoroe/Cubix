using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Helpers
{
    //Adding the "This" before Transform just means that it becomes an extension method of the Transform type.
    //Without the "This" before it, it can't be used anywhere in the script.
    //Using this we can now call this function on any intance of a transform.
    public static void CenterOnChildred(this Transform transformParent)
    {
        List<Transform> children = transformParent.Cast<Transform>().ToList();

        Vector3 pos = Vector3.zero;

        foreach (Transform C in children)
        {
            pos += C.position;
            C.parent = null;
        }

        pos /= children.Count;

        transformParent.position = pos;

        foreach (var C in children)
        {
            C.parent = transformParent;
        }
    }
}
