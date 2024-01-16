using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 slotSize;
    [SerializeField] private float heightSpacing;
    [SerializeField] private float widthSpacing;
    [SerializeField] private GameObject siteNodePrefab;

    private SiteNode[,] _grid;

    [ContextMenu("Generate")]
    public void GenerateGrid()
    {
        ClearGrid();
        _grid = new SiteNode[gridSize.x, gridSize.x];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                SiteNode tmpNode;
                Vector3 nodePos = GetRandomPosAtSlot(x, y);
                Instantiate(siteNodePrefab, nodePos, Quaternion.identity, transform).TryGetComponent<SiteNode>(out tmpNode);
                tmpNode.gridPos = new Vector2(x, y);
                _grid[x, y] = tmpNode;
            }
        }
    }

    private void OnValidate()
    {
        //limit grid and slot size editing
        if (gridSize.x <= 0) { gridSize.x = 1; }
        if (gridSize.y <= 0) { gridSize.y = 1; }

        if (slotSize.x <= 0) { slotSize.x = 0; }
        if (slotSize.y <= 0) { slotSize.y = 0; }
    }

    private Vector3 GetRandomPosAtSlot(int slotX, int slotY)
    {
        Vector3 resPos = new Vector3();

        resPos.x = widthSpacing * slotX;
        resPos.y = heightSpacing * slotY;

        resPos.x += Random.Range(-slotSize.x, slotSize.x);
        resPos.y += Random.Range(-slotSize.y, slotSize.y);

        return resPos;
    }

    [ContextMenu("ConnectNodes")]
    public void ConnectNodes()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                SiteNode currentNode = _grid[x, y];

                if (y > 0 && currentNode.entryLinks == 0)//delete unlinked nodes and skip rest of logic
                {
                    _grid[x, y] = null;
                    Destroy(currentNode.gameObject);
                    continue;
                }

                if (y < gridSize.y - 1)
                {

                    //get three nodes straight above current node
                    List<SiteNode> Upper3Nodes = new List<SiteNode>();
                    if (x > 0) { Upper3Nodes.Add(_grid?[x - 1, y + 1]); }
                    Upper3Nodes.Add(_grid?[x, y + 1]);
                    if (x < gridSize.x - 1) { Upper3Nodes.Add(_grid?[x + 1, y + 1]); }

                    //Choose one random node to link
                    SiteNode tmpNode = null;
                    tmpNode = Upper3Nodes[Random.Range(0, Upper3Nodes.Count)];
                    currentNode.CreateLink(tmpNode);
                    Upper3Nodes.Remove(tmpNode);

                    //try to link rest of nodes
                    foreach (var node in Upper3Nodes)
                    {
                        if (currentNode.LinksSum < currentNode.MaxLinks)
                        {
                            float rand = Random.Range(0, 1f);
                            if (rand <= node.chanceToLink)
                            {
                                currentNode.CreateLink(node);
                            }
                        }
                    }
                }
            }
        }
    }

    private void ClearGrid()
    {
        if (_grid != null)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (_grid[x, y])
                    {
                        Destroy(_grid[x, y].gameObject);
                    }
                }
            }
        }
    }
}
