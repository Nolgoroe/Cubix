using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameGrid : MonoBehaviour
{
    [Header("Generation Value Params")]
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float gridSpacing;
    [SerializeField] private float delayBetweenCellSpawn;
    [SerializeField] private Vector3 gridRotation;

    [Header("Generation PrefabParams")]
    [SerializeField] private GameObject gridCellPrefab;



    private GameObject [,] gameGridGameObjects;
    private GridCell [,] gameGridCells;

    void Start()
    {
        gameGridGameObjects = new GameObject[gridHeight, gridWidth];
        gameGridCells = new GridCell[gridHeight, gridWidth];

        StartCoroutine(CreateGrid());

    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            ClearGrid();

            StartCoroutine(CreateGrid());
        }
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

        gameGridGameObjects = new GameObject[gridHeight, gridWidth];
        gameGridCells = new GridCell[gridHeight, gridWidth];
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = Vector3.zero;
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

                if(gameGridGameObjects[x, y].TryGetComponent<GridCell>(out GridCell createdCell))
                {
                    gameGridCells[x,y] = createdCell;
                    createdCell.SetXYInGrid(x,y);
                }

                yield return new WaitForSeconds(delayBetweenCellSpawn);
            }
        }

        float lastCellX = gameGridGameObjects[gridHeight - 1, gridWidth - 1].transform.localPosition.x;
        float lastCellY = gameGridGameObjects[gridHeight - 1, gridWidth - 1].transform.localPosition.y;

        transform.position = new Vector3(-(lastCellX / 2), 0 , Camera.main.transform.position.y - (lastCellY / 2));
        transform.CenterOnChildred();

        transform.rotation = Quaternion.Euler(gridRotation.x, gridRotation.y, gridRotation.z);
    }

    #region Public Actions
    public void OverrideSpecificCell(Vector2Int posInArray, GridCell newCell, GameObject newObejct)
    {
        gameGridGameObjects[posInArray.x, posInArray.y] = newObejct;
        gameGridCells[posInArray.x, posInArray.y] = newCell;
    }

    public void SetGridParamsFromUI(int height, int width, int spacing)
    {
        gridHeight = height;
        gridWidth = width;
        gridSpacing = spacing;
    }
    #endregion

    #region Return Data
    public GridCell[,] ReturnCellsArray()
    {
        return gameGridCells;
    }
    #endregion
}
