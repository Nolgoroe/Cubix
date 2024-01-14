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

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                SiteNode tmpNode;
                Vector3 nodePos = GetRandomPosAtSlot(x, y);
                Instantiate(siteNodePrefab, nodePos, Quaternion.identity, transform).TryGetComponent<SiteNode>(out tmpNode);
                _grid[x, y] = tmpNode;
            }
        }
    }

    private void OnValidate()
    {
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
