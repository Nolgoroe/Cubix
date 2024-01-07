using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private float gridHeight;
    [SerializeField] private float gridWidth;
    [SerializeField] private float gridSpacing;
    [SerializeField] private List<GridCell> gameGridCellsList = new List<GridCell>();




    private GridCell[,] GridCellsArray;

    private void Start()
    {
        if (gameGridCellsList.Count > 0)
        {
            foreach (GridCell cell in gameGridCellsList)
            {
                int x = cell.ReturnPositionInGridArray().x;
                int y = cell.ReturnPositionInGridArray().y;
                GridCellsArray[x, y] = cell;
            }
        }

    }
























    public void AddCellToGridCellList(GridCell cell)
    {
        gameGridCellsList.Add(cell);
    }

    public void CopyOtherGrid(ToolGameGrid toolGameGrid)
    {
        Vector2 pos = toolGameGrid.ReturnGridWidthAndHeight();
        gridHeight = pos.y;
        gridWidth = pos.x;
        gridSpacing = toolGameGrid.ReturnSpacing();
    }
}
