using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float gridSpacing;
    [SerializeField] private float delayBetweenCellSpawn;
    [SerializeField] private float gridDistance = 10;
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject [,] gameGrid;

    void Start()
    {
        gameGrid = new GameObject[gridHeight, gridWidth];
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

        foreach (GameObject GO in gameGrid)
        {
            Destroy(GO);
        }

        Array.Clear(gameGrid, 0, gameGrid.Length);

        gameGrid = new GameObject[gridHeight, gridWidth];
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
                gameGrid[x, y] = Instantiate(gridCellPrefab, transform);
                gameGrid[x, y].name = "Grid Cell ( X: " + x.ToString() + " , Y: " + y.ToString() + ")";
                gameGrid[x, y].transform.localEulerAngles = Vector3.zero;
                gameGrid[x, y].transform.localPosition = new Vector3(x * gridSpacing, y * gridSpacing);

                yield return new WaitForSeconds(delayBetweenCellSpawn);
            }
        }

        float lastCellX = gameGrid[gridHeight - 1, gridWidth - 1].transform.localPosition.x;
        float lastCellY = gameGrid[gridHeight - 1, gridWidth - 1].transform.localPosition.y;

        transform.position = new Vector3(-(lastCellX / 2), 0 , Camera.main.transform.position.y - (lastCellY / 2));
    }
}
