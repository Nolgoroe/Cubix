using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteNode : MonoBehaviour
{
    [Header("BaseSettings")]
    public Vector2 gridPos;
    public RectTransform rect;
    [SerializeField] private GameObject mapLinePrefab;

    [Header("Progression")]
    public int entryLinks;
    public int exitLinks;
    [Range(0,1)]public float chanceToLink;
    public List<SiteNode> nextNodes = new List<SiteNode>();
    [SerializeField] private int maxLinks;
    private List<MapLine> lines = new List<MapLine>();

    #region Properties

    public int LinksSum { get { return exitLinks + entryLinks; } }
    public int MaxLinks { get { return maxLinks; } }

    #endregion

    public void LinkNode(SiteNode otherNode)
    {
        nextNodes.Add(otherNode);

    }

    public void CreateLink(SiteNode connectNode)
    {
        MapLine newLine = null;
        GameObject newLineGO = Instantiate(mapLinePrefab, transform.position, Quaternion.identity, transform);
        newLineGO.TryGetComponent<MapLine>(out newLine);
        newLine.Init(this, connectNode);
        newLine.RenderLine();
        connectNode.entryLinks++;
        exitLinks++;
        lines.Add(newLine);
    }
}
