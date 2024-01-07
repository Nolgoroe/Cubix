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

    [Header("Buildings")]
    [SerializeField] private Transform buildingParent;
    [SerializeField] private List<PlacedObject> placedObjectList;

    [Header("Enemies")]
    [SerializeField] private List<ToolEnemySpawnerCell> enemySpawners;
    [SerializeField] private List<ToolEnemyPathCell> enemyPathCells;

    private GameObject [,] toolGridGameObjects;
    private ToolGridCell [,] toolGridCells;


    private void OnValidate()
    {
        if(refreshMaterials)
        {
            UpdateEachCellsMaterial();
            refreshMaterials = false;
        }
    }
    void Awake()
    {
        //These do not serialize = will not save in prefab, so we use the serialized and saved list in order to load the level data
        toolGridGameObjects = new GameObject[gridWidth, gridHeight];
        toolGridCells = new ToolGridCell[gridWidth, gridHeight];
        levelCreationTool = ToolReferencerObject.Instance.levelCreationToolSO;
        enemySpawners = new List<ToolEnemySpawnerCell>();
        enemyPathCells = new List<ToolEnemyPathCell>();

        if (gameGridCellsList.Count > 0)
        {
            foreach (ToolGridCell cell in gameGridCellsList)
            {
                int x = cell.ReturnPosInGridArray().x;
                int y = cell.ReturnPosInGridArray().y;
                toolGridCells[x, y] = cell;
                toolGridGameObjects[x, y] = cell.gameObject;

            }
        }
        else
        {
            gameGridCellsList = new List<ToolGridCell>();
        }




        Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);

        refreshMaterials = true;
    }

    public void InitNewGrid()
    {
        ClearGrid();

        StartCoroutine(CreateGrid());
        //CreateGrid();
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

        Array.Clear(toolGridGameObjects, 0, toolGridGameObjects.Length);
        Array.Clear(toolGridCells, 0, toolGridCells.Length);

        toolGridGameObjects = new GameObject[gridWidth, gridHeight];
        toolGridCells = new ToolGridCell[gridWidth, gridHeight];
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = Vector3.zero;

        Camera.main.transform.position = Vector3.zero;
        Camera.main.transform.rotation = Quaternion.Euler(45,0,0);

        gameGridCellsList.Clear();
        enemyPathCells.Clear();
        enemySpawners.Clear();
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
            StartCoroutine(enemySpawner.ClearAllTempData());
        }
    }

    [ContextMenu("Generate The Level")]
    public void GenerateTheLevel()
    {
        ClearDataBeforeLevelGeneration();
        CleanupBeforePrefab();

        foreach (ToolGridCell cell in toolGridCells)
        {
            //find the prefab we want to spawn and it's cell type
            GameObject toSpawn = ToolReferencerObject.Instance.levelCreationToolSO.SpawnPrefabByColor(cell.ReturnCellColor());
            toSpawn.TryGetComponent<ToolGridCell>(out ToolGridCell tempCell);

            //if the prefab we want to spawn is null or if the types are the same, we don't do anything to this cell.
            if (toSpawn == null || cell.ReturnTypeOfCell() == tempCell.ReturnTypeOfCell()) continue;

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

    private IEnumerator CreateGrid()
    {
        if(gridCellPrefab == null)
        {
            Debug.LogError("Must have prefab!");
            yield break;
        }

        transform.name = "New Level";

        yield return new WaitForEndOfFrame(); // we do this to let the data clear from the destroy before continue
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                toolGridGameObjects[x, y] = Instantiate(gridCellPrefab, cellsParent);
                toolGridGameObjects[x, y].name = "Grid Cell ( X: " + x.ToString() + " , Y: " + y.ToString() + ")";
                toolGridGameObjects[x, y].transform.localEulerAngles = Vector3.zero;
                toolGridGameObjects[x, y].transform.localPosition = new Vector3(x * gridSpacing, y * gridSpacing);

                if(toolGridGameObjects[x, y].TryGetComponent<ToolGridCell>(out ToolGridCell createdCell))
                {
                    toolGridCells[x,y] = createdCell;
                    createdCell.SetXYInGrid(x,y);

                    gameGridCellsList.Add(createdCell);
                }

                //yield return new WaitForSeconds(delayBetweenCellSpawn);
            }
        }

        float lastCellX = toolGridGameObjects[gridWidth - 1, gridHeight - 1].transform.localPosition.x;
        float lastCellY = toolGridGameObjects[gridWidth - 1, gridHeight - 1].transform.localPosition.y;

        transform.position = new Vector3(-(lastCellX / 2), 0 , Camera.main.transform.position.y - (lastCellY / 2));
        transform.CenterOnChildred();

        Vector3 camPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(camPos.x, transform.position.y + 20, transform.position.z - 20);

        transform.rotation = Quaternion.Euler(gridRotation.x, gridRotation.y, gridRotation.z);
    }

    #region Public Actions
    private void OverrideSpecificCell(Vector2Int posInArray, ToolGridCell newCell, GameObject newObejct)
    {
        toolGridGameObjects[posInArray.x, posInArray.y] = newObejct;
        toolGridCells[posInArray.x, posInArray.y] = newCell;


        //gameGridCellsList.Add(newCell);
    }

    public void SetGridParamsFromUI(int height, int width, int spacing)
    {
        gridHeight = height;
        gridWidth = width;
        gridSpacing = spacing;
    }
    #endregion

    #region Return Data
    public ToolGridCell[,] ReturnCellsArray()
    {
        return toolGridCells;
    }

    public Vector2 ReturnGridWidthAndHeight()
    {
        return new Vector2(gridWidth, gridHeight);
    }
    public Transform ReturnBuildingsParent()
    {
        return buildingParent;
    }
    public Transform ReturnWaypointsParent()
    {
        return waypointsParent;
    }
    #endregion


    private void UpdateEachCellsMaterial()
    {
        foreach (ToolGridCell cell in gameGridCellsList)
        {
            cell.PermaChangeMat(levelCreationTool.ReturnMatByType(cell.ReturnTypeOfCell()));
        }
    }

    public void AddRemoveToPlacedObjectList(bool add, PlacedObject placedObject)
    {
        if(add)
        {
            placedObjectList.Add(placedObject);
        }
        else
        {
            if(placedObjectList.Contains(placedObject))
            {
                placedObjectList.Remove(placedObject);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Make Game Level Prefab")]
    private void CreatePrefabFromGridTool()
    {
        Debug.Log("Creating game level now");
        //this is where we start changing all the data of every object under this level parent.

        foreach (ToolEnemySpawnerCell toolEnemySpawner in enemySpawners)
        {
            // create the game enemy spawner SCRIPT
            // transfer all relavent data FROM tool enemy spawner TO game enemy spawner SCRIPT
            // finished enemy spawners.
        }

        foreach (ToolGridCell toolGridCell in gameGridCellsList)
        {
            // create the game cell object SCRIPT by it's type - normal cell type, player base type, obstacle type, etc...
            // transfer all relavent data FROM tool grid cell TO new cell SCRIPT
            // finished grid cells.
        }

        foreach (PlacedObject placedObject in placedObjectList)
        {
            //clear all scripts and colliders from each building.

            //remove all colliders from object and it's childern.
            Collider[] colList = transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colList)
            {
                DestroyImmediate(collider);
            }

            DestroyImmediate(placedObject);
        }


    }
#endif

}
