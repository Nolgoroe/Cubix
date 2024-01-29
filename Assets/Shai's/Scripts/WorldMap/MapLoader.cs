using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//********this class only works with preloaded map*********
public class MapLoader : MonoBehaviour
{
    public List<NodeData> nodesData;

    public void SaveData(List<SiteNode> nodes)
    {
        foreach (var node in nodes)
        {
            //nodesData.Add(node.ExportData());
        }
    }

    public void LoadData(List<SiteNode> nodes)
    {
        //foreach (var item in collection)
        //{

        //}
    }
}

public class NodeData
{
    public bool isComplete;
    public bool isLocked;

    public NodeData(bool _isComplete, bool _isLocked)
    {
        isComplete = _isComplete;
        isLocked = _isLocked;
    }
}
