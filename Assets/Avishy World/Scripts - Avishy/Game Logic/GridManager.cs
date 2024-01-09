using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float gridSpacing;
    [SerializeField] private List<GridCell> gameGridCellsList = new List<GridCell>();
    [SerializeField] private List<EnemySpawnerCell> enemySpawnerCells;

    private GridCell[,] GridCellsArray;

    private void Awake()
    {
        Instance = this;

        GridCellsArray = new GridCell[gridWidth, gridHeight];

        if (gameGridCellsList.Count > 0)
        {
            foreach (GridCell cell in gameGridCellsList)
            {
                int x = cell.ReturnPositionInGridArray().x;
                int y = cell.ReturnPositionInGridArray().y;
                GridCellsArray[x, y] = cell;
            }
        }

        InitCellsData();
    }

    private void InitCellsData()
    {
        foreach (GridCell gridCell in gameGridCellsList)
        {
            switch (gridCell.ReturnTypeOfCell())
            {
                case TypeOfCell.enemyPath:
                    break;
                case TypeOfCell.enemySpawner:
                    EnemySpawnerCell spawnerCell = gridCell as EnemySpawnerCell;
                    spawnerCell.InitWaypointData();

                    enemySpawnerCells.Add(spawnerCell);
                    break;
                case TypeOfCell.Obstacle:
                    break;
                case TypeOfCell.PlayerBase:
                    break;
                case TypeOfCell.None:
                    break;
                default:
                    break;
            }
        }
    }



















    public GridCell ReturnCellFromList(Vector2Int vector)
    {
        int cellToSwapIndex = gameGridCellsList.IndexOf(gameGridCellsList.Where(x => x.ReturnPositionInGridArray() == vector).FirstOrDefault());

        if (cellToSwapIndex < 0)
        {
            Debug.LogError("Error!");
        }

        return gameGridCellsList[cellToSwapIndex];
    }

    public GridCell ReturnCellAtVector(Vector2Int vector)
    {
        return GridCellsArray[vector.x, vector.y];
    }
    public List<EnemySpawnerCell> ReturnLevelEnemySpawners()
    {
        return enemySpawnerCells;
    }
    public Vector2Int ReturnWidthHeight()
    {
        return new Vector2Int(gridWidth, gridHeight);
    }




    public void AddCellToGridCellList(GridCell cell)
    {
        gameGridCellsList.Add(cell);
    }


    public void CopyOtherGrid(ToolGameGrid toolGameGrid)
    {
        Vector2 pos = toolGameGrid.ReturnGridWidthAndHeight();
        gridHeight = (int)pos.y;
        gridWidth = (int)pos.x;
        gridSpacing = toolGameGrid.ReturnSpacing();
    }
}
