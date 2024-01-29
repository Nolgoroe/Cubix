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
            nodesData.Add(node.ExportData());
        }
    }

    public void LoadData(List<SiteNode> nodes)
    {
        foreach (var node in nodes)
        {
            foreach (var data in nodesData)
            {
                if (node.ID == data.id)
                {

                }
            }
        }
    }
}

public class NodeData
{
    public bool isComplete;
    public bool isLocked;
    public string id;

    public NodeData(bool _isComplete, bool _isLocked, string _id)
    {
        isComplete = _isComplete;
        isLocked = _isLocked;
        id = _id;
    }
}
