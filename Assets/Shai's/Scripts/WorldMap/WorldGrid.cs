using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridDirection { BottomUp, LeftRight }

public class WorldGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 slotSize;
    [SerializeField] private Vector2 gridOriginOffset;
    [SerializeField] private float heightSpacing;
    [SerializeField] private float widthSpacing;
    [SerializeField] private GameObject siteNodePrefab;
    [SerializeField] private LinesParent linesParent;
    [SerializeField] private MapProgression progression;
    [SerializeField] private MapLoader loader; 
    [SerializeField] private GridDirection direction;
    [SerializeField] private List<SiteNode> nodes;

    [Header("Node Linking")]
    [SerializeField] private int maxSinngleLinkStreak;

    private SiteNode[,] _grid;


    private void Start()
    {
        if (nodes == null)
        {
            CreateWorldMap();
        }
        else
        {
            _grid = new SiteNode[gridSize.x, gridSize.y];
            foreach (var node in nodes)
            {
                _grid[node.gridPos.x, node.gridPos.y] = node;
            }
            progression.Init();
        }

        SubscribeToNodes();
        LoadMap();
    }

    public void CreateWorldMap()
    {
        GenerateGrid();
        ConnectNodes();
        linesParent.AdoptLines();
        progression.Init();
    }


    [ContextMenu("GenerateLines")]
    public void UpdateLines()
    {
        foreach (var node in nodes)
        {
            node.CreateLinks();
        }

        linesParent.AdoptLines();
    }


    private void GenerateGrid()
    {
        ClearGrid();
        _grid = new SiteNode[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                SiteNode tmpNode;
                Vector3 nodePos = GetPosAtSlot(x, y);
                Instantiate(siteNodePrefab, nodePos, Quaternion.identity, transform).TryGetComponent<SiteNode>(out tmpNode);
                tmpNode.gridPos = new Vector2Int(x, y);
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

    private Vector3 GetPosAtSlot(int slotX, int slotY)
    {
        //add modifiers for grid generation direction
        Vector2 gridOrigin;
        Vector3 resPos = new Vector3();

        switch (direction)
        {
            case GridDirection.BottomUp:
                resPos.x = widthSpacing * slotX + gridOriginOffset.x;
                resPos.y = heightSpacing * slotY + gridOriginOffset.y;
                break;
            case GridDirection.LeftRight:
                resPos.x = widthSpacing * slotY + gridOriginOffset.y;
                resPos.y = heightSpacing * slotX + gridOriginOffset.x;
                break;
            default:
                break;
        }



        //resPos.x = widthSpacing * slotX + gridOriginOffset.x;
        //resPos.y = heightSpacing * slotY + gridOriginOffset.y;

        if (slotY > 0) //dont add random offset to first tier
        {
            resPos.x += Random.Range(-slotSize.x, slotSize.x);
            resPos.y += Random.Range(-slotSize.y, slotSize.y);
        }

        return resPos;
    }

    private void ConnectNodes()
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

                    if (x > 0 && _grid[x - 1, y]
                        && !_grid[x - 1, y].nextNodes.Contains(_grid[x, y + 1])) //this makes sure that there will be no line crossing
                    {
                        Upper3Nodes.Add(_grid[x - 1, y + 1]);
                    }

                    Upper3Nodes.Add(_grid[x, y + 1]);
                    if (x < gridSize.x - 1) { Upper3Nodes.Add(_grid[x + 1, y + 1]); }


                    //Choose one random node to link
                    SiteNode tmpNode = null;
                    tmpNode = Upper3Nodes[Random.Range(0, Upper3Nodes.Count)];
                    currentNode.CreateLink(tmpNode);
                    Upper3Nodes.Remove(tmpNode);

                    //if node's singleLinkStreak exceeds max, create another link
                    if (currentNode.singleLinkStreak > maxSinngleLinkStreak && Upper3Nodes.Count > 0)
                    {
                        tmpNode = Upper3Nodes[Random.Range(0, Upper3Nodes.Count)];
                        currentNode.CreateLink(tmpNode);
                        Upper3Nodes.Remove(tmpNode);
                    }

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
                    //increase singleLinkStreak if needed
                    if (currentNode.exitLinks == 1)
                    {
                        SiteNode nextNode = currentNode.nextNodes[0];
                        if (currentNode.singleLinkStreak == 0)
                        {
                            nextNode.singleLinkStreak = 1;
                        }
                        else
                        {
                            nextNode.singleLinkStreak += currentNode.singleLinkStreak;
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

    public List<SiteNode> GetAllNodes()
    {
        List<SiteNode> allNodes = new List<SiteNode>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_grid[x, y] != null)
                {
                    allNodes.Add(_grid[x, y]);
                }
            }
        }

        return allNodes;
    }

    [ContextMenu("Save")]
    public void SaveMap()
    {
        if (Player.Instance)
        {
            Player.Instance.SaveMapProgression(GetAllNodes());
        }
    }

    [ContextMenu("Load")]
    public void LoadMap()
    {
        if (Player.Instance)
        {
            Player.Instance.LoadMapProgression(GetAllNodes());

            //sync progression
            List<SiteNode> openNodes = new List<SiteNode>();
            foreach (var node in GetAllNodes())
            {
                if (!node.isLocked)
                {
                    openNodes.Add(node);
                }
            }
            progression.SetOpenNodes(openNodes);
        }
    }

    private void SubscribeToNodes()
    {
        foreach (var node in GetAllNodes())
        {
            node.button.onClick.AddListener(SaveMap);
        }
    }

}
