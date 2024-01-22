using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesParent : MonoBehaviour
{
    [SerializeField] private WorldGrid nodeGrid;

    public void AdoptLines()
    {
        foreach (var node in nodeGrid.GetAllNodes())
        {
            node.ChangeLinesParent(transform);
        }
    }
}
