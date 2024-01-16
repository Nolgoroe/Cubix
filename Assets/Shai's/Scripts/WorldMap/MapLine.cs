using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLine : MonoBehaviour
{
    [SerializeField] private UILineRenderer lRenderer;
    private SiteNode originNode;
    private SiteNode connectedNode;

    public void Init(SiteNode _originNode, SiteNode _connectedNode)
    {
        originNode = _originNode;
        connectedNode = _connectedNode;
    }
    public void RenderLine()
    {
        Vector2 pointToConnect = connectedNode.rect.anchoredPosition - originNode.rect.anchoredPosition;
        lRenderer.points.Add(pointToConnect);
    }
}
