using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ToolGameGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelCreationToolSO levelCreationTool; // we keep this here since we need a reference to it in the prefab

    [Header("Generation Value Params")]
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float gridSpacing;
    [SerializeField] private float delayBetweenCellSpawn;
    [SerializeField] private Vector3 gridRotation;
    [SerializeField] private List<ToolGridCell> gameGridCellsList;
    [SerializeField] private Transform cellsParent;

    [Header("Generation Prefab Params")]
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private bool refreshMaterials;

    [Header("Waypoints")]
    [SerializeField] private Transform waypointsParent;

    [Header("Parents")]
    [SerializeField] private Transform buildingParent;
    [SerializeField] private Transform gamePartsParent;
    [SerializeField] private Transform enemyPathParent;
    [SerializeField] private Transform towerSlotsParent;

    [Header("Enemies")]
    [SerializeField] private List<ToolEnemySpawnerCell> enemySpawners;
    [SerializeField] private List<ToolEnemyPathCell> enemyPathCells;

    private GameObject [,] toolGridGameObjectsArray;
    private ToolGridCell [,] toolGridCellsArray;
    private List<PlacedObject> placedObjectList;


    private void OnValidate()
    {
        if(refreshMaterials)
        {
            UpdateEachCellsMaterial();
            refreshMaterials = false;
        }
    }
    private void Awake()
    {
        //These do not serialize = will not save in prefab, so we use the serialized and saved list in order to load the level data
        toolGridGameObjectsArray = new GameObject[gridWidth, gridHeight];
        toolGridCellsArray = new ToolGridCell[gridWidth, gridHeight];
        levelCreationTool = ToolReferencerObject.Instance.levelCreationToolSO;

        if(enemySpawners == null || enemySpawners.Count == 0)
        {
            enemySpawners = new List<ToolEnemySpawnerCell>();
        }

        if(enemyPathCells == null || enemyPathCells.Count == 0)
        {
            enemyPathCells = new List<ToolEnemyPathCell>();
        }

        if (gameGridCellsList.Count > 0)
        {
            foreach (ToolGridCell cell in gameGridCellsList)
            {
                int x = cell.ReturnPosInGridArray().x;
                int y = cell.ReturnPosInGridArray().y;
                toolGridCellsArray[x, y] = cell;
                toolGridGameObjectsArray[x, y] = cell.gameObject;

            }
        }
        else
        {
            gameGridCellsList = new List<ToolGridCell>();
        }




        Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);

        refreshMaterials = true;
    }

    private void Start()
    {
        placedObjectList = new List<PlacedObject>();
    }

    private IEnumerator CreateGrid()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("Must have prefab!");
            yield break;
        }

        if (gridWidth <= 0 || gridHeight <= 0 || gridSpacing <= 0)
        {
            ToolReferencerObject.Instance.toolUI.CallDisplaySystemMessage("Must set Height, Width and Spacing to values more than 0!");
            yield break;
        }

        transform.name = "New Level";

        yield return new WaitForEndOfFrame(); // we do this to let the data clear from the destroy before continue
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                toolGridGameObjectsArray[x, y] = Instantiate(gridCellPrefab, cellsParent);
                toolGridGameObjectsArray[x, y].name = "Grid Cell ( X: " + x.ToString() + " , Y: " + y.ToString() + ")";
                toolGridGameObjectsArray[x, y].transform.localEulerAngles = Vector3.zero;
                toolGridGameObjectsArray[x, y].transform.localPosition = new Vector3(x * gridSpacing, y * gridSpacing);

                if (toolGridGameObjectsArray[x, y].TryGetComponent<ToolGridCell>(out ToolGridCell createdCell))
                {
                    toolGridCellsArray[x, y] = createdCell;
                    createdCell.SetXYInGrid(x, y);

                    gameGridCellsList.Add(createdCell);
                }

                //yield return new WaitForSeconds(delayBetweenCellSpawn);
            }
        }

        float lastCellX = toolGridGameObjectsArray[gridWidth - 1, gridHeight - 1].transform.localPosition.x;
        float lastCellY = toolGridGameObjectsArray[gridWidth - 1, gridHeight - 1].transform.localPosition.y;

        transform.position = new Vector3(-(lastCellX / 2), 0, Camera.main.transform.position.y - (lastCellY / 2));
        Helpers.CenterOnChildred(transform);
        //transform.CenterOnChildred();

        Vector3 camPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(camPos.x, transform.position.y + 20, transform.position.z - 20);

        transform.rotation = Quaternion.Euler(gridRotation.x, gridRotation.y, gridRotation.z);
    }

    private void ClearGrid()
    {
        StopAllCoroutines();

        foreach (Transform child in cellsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in buildingParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in waypointsParent)
        {
            Destroy(child.gameObject);
        }

        Array.Clear(toolGridGameObjectsArray, 0, toolGridGameObjectsArray.Length);
        Array.Clear(toolGridCellsArray, 0, toolGridCellsArray.Length);

        toolGridGameObjectsArray = new GameObject[gridWidth, gridHeight];
        toolGridCellsArray = new ToolGridCell[gridWidth, gridHeight];
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = Vector3.zero;

        Camera.main.transform.position = Vector3.zero;
        Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);

        gameGridCellsList.Clear();
        enemyPathCells.Clear();
        enemySpawners.Clear();
    }

    private void OverrideSpecificCell(Vector2Int posInArray, ToolGridCell newCell, GameObject newObejct)
    {
        toolGridGameObjectsArray[posInArray.x, posInArray.y] = newObejct;
        toolGridCellsArray[posInArray.x, posInArray.y] = newCell;


        //gameGridCellsList.Add(newCell);
    }

    private void SwapDataFromNewCreation(ToolGridCell cell, ToolGridCell createdCell)
    {
        //remove previous cell data from lists and such
        switch (cell.ReturnTypeOfCell())
        {
            case TypeOfCell.enemyPath:
                enemyPathCells.Remove(cell as ToolEnemyPathCell);
                break;
            case TypeOfCell.enemySpawner:
                enemySpawners.Remove(cell as ToolEnemySpawnerCell);
                break;
            case TypeOfCell.Obstacle:
                break;
            case TypeOfCell.PlayerBase:
                break;
            case TypeOfCell.None:
                break;
            case TypeOfCell.Waypoints:
                break;
            default:
                break;
        }

        // Add new cell created to cells and sucl
        switch (createdCell.ReturnTypeOfCell())
        {
            case TypeOfCell.enemyPath:
                enemyPathCells.Add(createdCell as ToolEnemyPathCell);
                break;
            case TypeOfCell.enemySpawner:
                enemySpawners.Add(createdCell as ToolEnemySpawnerCell);
                break;
            case TypeOfCell.Obstacle:
                break;
            case TypeOfCell.PlayerBase:
                break;
            case TypeOfCell.None:
                break;
            case TypeOfCell.Waypoints:
                break;
            default:
                break;
        }
    }

    private void UpdateEachCellsMaterial()
    {
        foreach (ToolGridCell cell in gameGridCellsList)
        {
            cell.PermaChangeMat(levelCreationTool.ReturnMatByType(cell.ReturnTypeOfCell()));
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Make Game Level Prefab")]
    private void CreatePrefabFromGridTool()
    {
        Debug.Log("Creating game level now");

        GridManager gridManager = new GridManager();
        gridManager = gameObject.AddComponent<GridManager>();
        gridManager.CopyOtherGrid(this);

        foreach (ToolGridCell toolGridCell in gameGridCellsList)
        {
            switch (toolGridCell.ReturnTypeOfCell())
            {
                case TypeOfCell.enemyPath:
                    GridCell enemyPathCell = new GridCell();
                    enemyPathCell = toolGridCell.gameObject.AddComponent<GridCell>();
                    enemyPathCell.CopyDataFromToolCell(toolGridCell);

                    gridManager.AddCellToGridCellList(enemyPathCell);

                    enemyPathCell.transform.SetParent(enemyPathParent);
                    break;
                case TypeOfCell.enemySpawner:
                    ToolEnemySpawnerCell toolSpawnerCell = toolGridCell.GetComponent<ToolEnemySpawnerCell>();
                    toolSpawnerCell.DestroySpawnerNumberText();

                    EnemySpawnerCell spawnerCell = new EnemySpawnerCell();
                    spawnerCell = toolGridCell.gameObject.AddComponent<EnemySpawnerCell>();

                    spawnerCell.CopyDataFromToolCell(toolGridCell);

                    gridManager.AddCellToGridCellList(spawnerCell);

                    spawnerCell.transform.SetParent(gamePartsParent);
                    break;
                case TypeOfCell.Obstacle:
                    GridCell obstacleGridCell = new GridCell();
                    obstacleGridCell = toolGridCell.gameObject.AddComponent<GridCell>();
                    obstacleGridCell.CopyDataFromToolCell(toolGridCell);

                    gridManager.AddCellToGridCellList(obstacleGridCell);

                    obstacleGridCell.transform.SetParent(gamePartsParent);
                    break;
                case TypeOfCell.PlayerBase:
                    //This is where we add player home base logic
                    PlayerHomeBaseCell playerBaseCell = new PlayerHomeBaseCell();
                    playerBaseCell = toolGridCell.gameObject.AddComponent<PlayerHomeBaseCell>();
                    playerBaseCell.CopyDataFromToolCell(toolGridCell);

                    gridManager.AddCellToGridCellList(playerBaseCell);
                    playerBaseCell.transform.SetParent(gamePartsParent);

                    break;
                case TypeOfCell.None:
                    //This is where we add normal game cell logic
                    GridCell gridCell = new GridCell();
                    gridCell = toolGridCell.gameObject.AddComponent<GridCell>();
                    gridCell.CopyDataFromToolCell(toolGridCell);

                    gridManager.AddCellToGridCellList(gridCell);

                    if (gridCell.ReturnCellTypeColor() != CellTypeColor.None)
                    {
                        gridCell.transform.SetParent(towerSlotsParent);
                        gridManager.AddCellToTowerBaseCells(gridCell);
                    }
                    break;
                default:
                    break;
            }

            DestroyImmediate(toolGridCell, true);
        }

        if (placedObjectList != null)
        {
            foreach (PlacedObject placedObject in placedObjectList)
            {
                if (placedObject == null) continue;

                //clear all scripts and colliders from each building.

                //remove all colliders from object and it's childern.
                Collider[] colList = placedObject.transform.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colList)
                {
                    DestroyImmediate(collider);
                }

                DestroyImmediate(placedObject);
            }
        }

        EditorUtility.SetDirty(gridManager.gameObject);

        CleanMainParent();

        DestroyImmediate(this, true);
    }
#endif

    private void CleanMainParent()
    {
        DestroyImmediate(waypointsParent.gameObject, true);
    }





    #region Public Actions

    public void SetGridParamsFromUI(int height, int width, int spacing)
    {
        gridHeight = height;
        gridWidth = width;
        gridSpacing = spacing;
    }

    public void InitNewGrid()
    {
        ClearGrid();

        StartCoroutine(CreateGrid());
    }

    public void ClearDataBeforeLevelGeneration()
    {
        //we don't add the 2D arrays here since we ovveride their data in the "OverrideSpecificCell" funciton
        //gameGridCellsList.Clear();

    }

    public void CleanupBeforePrefab()
    {
        foreach (Transform child in waypointsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ToolEnemySpawnerCell enemySpawner in enemySpawners)
        {
            StartCoroutine(enemySpawner.ClearAllData());
        }
    }

    [ContextMenu("Generate The Level")]
    public void GenerateTheLevel()
    {
        ClearDataBeforeLevelGeneration();
        CleanupBeforePrefab();

        foreach (ToolGridCell cell in toolGridCellsArray)
        {
            //find the prefab we want to spawn and it's cell type
            GameObject toSpawn = ToolReferencerObject.Instance.levelCreationToolSO.SpawnPrefabByColor(cell.ReturnCellColor());
            toSpawn.TryGetComponent<ToolGridCell>(out ToolGridCell localCell);

            //if the prefab we want to spawn is null or if the types are the same, we don't do anything to this cell.
            if (toSpawn == null || cell.ReturnTypeOfCell() == localCell.ReturnTypeOfCell()) continue;

            int cellToSwapIndex = gameGridCellsList.IndexOf(gameGridCellsList.Where(x => x == cell).FirstOrDefault());


            GameObject newObject = Instantiate(toSpawn, cellsParent);
            newObject.TryGetComponent<ToolGridCell>(out ToolGridCell createdCell);

            if (createdCell != null)
            {
                createdCell.CopyOtherGridCell(cell);

                gameGridCellsList[cellToSwapIndex] = createdCell;
            }

            newObject.transform.localPosition = new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, toSpawn.transform.position.z);

            OverrideSpecificCell(cell.ReturnPosInGridArray(), createdCell, newObject);
            createdCell.ChangeMat(ToolReferencerObject.Instance.levelCreationToolSO.ReturnMatByType(createdCell.ReturnTypeOfCell()));

            Destroy(cell.gameObject);

            SwapDataFromNewCreation(cell, createdCell);
        }


        foreach (ToolEnemyPathCell enemyPathCell in enemyPathCells)
        {
            enemyPathCell.DecideOnPathMesh();
        }

        for (int i = 0; i < enemySpawners.Count; i++)
        {
            enemySpawners[i].name = "Enemy Spawner " + i;
            enemySpawners[i].SetSpawnerNumberText(i);
        }
    }

    public void AddRemoveToPlacedObjectList(bool add, PlacedObject placedObject)
    {
        if (add)
        {
            placedObjectList.Add(placedObject);
        }
        else
        {
            if (placedObjectList.Contains(placedObject))
            {
                placedObjectList.Remove(placedObject);
            }
        }
    }

    #endregion

    #region Return Data
    public ToolGridCell[,] ReturnCellsArray()
    {
        return toolGridCellsArray;
    }

    public Vector2 ReturnGridWidthAndHeight()
    {
        return new Vector2(gridWidth, gridHeight);
    }
    public float ReturnSpacing()
    {
        return gridSpacing;
    }
    public Transform ReturnBuildingsParent()
    {
        return buildingParent;
    }
    public Transform ReturnWaypointsParent()
    {
        return waypointsParent;
    }
    public List<ToolEnemySpawnerCell> ReturnEnemySpawners()
    {
        return enemySpawners;
    }
    #endregion




}
