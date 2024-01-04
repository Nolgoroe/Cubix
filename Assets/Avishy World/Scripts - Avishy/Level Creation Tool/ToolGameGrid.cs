using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ToolGameGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelCreationToolSO levelCreationToolColors; // we keep this here since we need a reference to it in the prefab
    [SerializeField] private ToolReferencerObject referencer; // we keep this here since we need a reference to it in the prefab

    [Header("Generation Value Params")]
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float gridSpacing;
    [SerializeField] private float delayBetweenCellSpawn;
    [SerializeField] private Vector3 gridRotation;
    [SerializeField] private List<ToolGridCell> gameGridCellsList;

    [Header("Generation Prefab Params")]
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private bool refreshMaterials;



    private GameObject [,] gameGridGameObjects;
    private ToolGridCell [,] gameGridCells;


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
        referencer = FindObjectOfType<ToolReferencerObject>();

        //These do not serialize = will not save in prefab, so we use the serialized and saved list in order to load the level data
        gameGridGameObjects = new GameObject[gridWidth, gridHeight];
        gameGridCells = new ToolGridCell[gridWidth, gridHeight];
        levelCreationToolColors = referencer.levelCreationToolSO;

        if (gameGridCellsList.Count > 0)
        {
            foreach (ToolGridCell cell in gameGridCellsList)
            {
                int x = cell.ReturnPosInGridArray().x;
                int y = cell.ReturnPosInGridArray().y;
                gameGridCells[x, y] = cell;
                gameGridGameObjects[x, y] = cell.gameObject;

            }
        }
        else
        {
            gameGridCellsList = new List<ToolGridCell>();
        }





        refreshMaterials = true;
    }

    public void InitNewGrid()
    {
        ClearGrid();

        StartCoroutine(CreateGrid());
    }

    private void ClearGrid()
    {
        StopAllCoroutines();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Array.Clear(gameGridGameObjects, 0, gameGridGameObjects.Length);
        Array.Clear(gameGridCells, 0, gameGridCells.Length);

        gameGridGameObjects = new GameObject[gridWidth, gridHeight];
        gameGridCells = new ToolGridCell[gridWidth, gridHeight];
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = Vector3.zero;
    }

    public void ClearDataBeforeLevelGeneration()
    {
        //we don't add the 2D arrays here since we ovveride their data in the "OverrideSpecificCell" funciton
        gameGridCellsList.Clear();
    }

    private IEnumerator CreateGrid()
    {
        if(gridCellPrefab == null)
        {
            Debug.LogError("Must have prefab!");
            yield break;
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                gameGridGameObjects[x, y] = Instantiate(gridCellPrefab, transform);
                gameGridGameObjects[x, y].name = "Grid Cell ( X: " + x.ToString() + " , Y: " + y.ToString() + ")";
                gameGridGameObjects[x, y].transform.localEulerAngles = Vector3.zero;
                gameGridGameObjects[x, y].transform.localPosition = new Vector3(x * gridSpacing, y * gridSpacing);

                if(gameGridGameObjects[x, y].TryGetComponent<ToolGridCell>(out ToolGridCell createdCell))
                {
                    gameGridCells[x,y] = createdCell;
                    createdCell.SetXYInGrid(x,y);
                }

                yield return new WaitForSeconds(delayBetweenCellSpawn);
            }
        }

        float lastCellX = gameGridGameObjects[gridWidth - 1, gridHeight - 1].transform.localPosition.x;
        float lastCellY = gameGridGameObjects[gridWidth - 1, gridHeight - 1].transform.localPosition.y;

        transform.position = new Vector3(-(lastCellX / 2), 0 , Camera.main.transform.position.y - (lastCellY / 2));
        transform.CenterOnChildred();

        transform.rotation = Quaternion.Euler(gridRotation.x, gridRotation.y, gridRotation.z);
    }

    #region Public Actions
    public void OverrideSpecificCell(Vector2Int posInArray, ToolGridCell newCell, GameObject newObejct)
    {
        gameGridGameObjects[posInArray.x, posInArray.y] = newObejct;
        gameGridCells[posInArray.x, posInArray.y] = newCell;


        gameGridCellsList.Add(newCell);
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
        return gameGridCells;
    }
    #endregion


    private void UpdateEachCellsMaterial()
    {
        foreach (ToolGridCell cell in gameGridCellsList)
        {
            cell.ChangeMat(levelCreationToolColors.ReturnMatByType(cell.ReturnTypeOfCell()));
        }
    }
}
