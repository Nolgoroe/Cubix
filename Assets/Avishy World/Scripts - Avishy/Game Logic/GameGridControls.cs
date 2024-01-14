using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridControls : MonoBehaviour
{
    [Header("Preset Data")]
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private GameObject currentTowerPrefab;// temp here - think of a place to hold all the towers available and choose.
    [SerializeField] private List<GameObject> towerPrefabs;// temp here - used for checks
    [SerializeField] private int indexInTowersList;// temp here - used for checks

    [Header("Automated data")]
    [SerializeField] private GridCell currentCellHovered;



    private Vector3 positionOfMouse;



    private void Start()
    {
        currentTowerPrefab = towerPrefabs[indexInTowersList];

        UIManager.Instance.SetTowerText(currentTowerPrefab.name); //Temp
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            indexInTowersList++;
            if(indexInTowersList >= towerPrefabs.Count)
            {
                indexInTowersList = 0;
            }
            currentTowerPrefab = towerPrefabs[indexInTowersList];

            UIManager.Instance.SetTowerText(currentTowerPrefab.name); //Temp
        }

        MouseOverGridCell();

        if (currentCellHovered)
        {
            NormalControls();
        }
    }

    private void NormalControls()
    {
        if (Input.GetMouseButton(0))
        {
            if(!currentCellHovered.ReturnIsOccipied())
            {
                if (!CheckCanPlaceTowerOnCell()) return;

                InstantiateTower();
            }
        }

        if (Input.GetMouseButton(1))
        {
            if(currentCellHovered.ReturnIsOccipied() && currentCellHovered.ReturnIsOccipiedByTower())
            {
                currentCellHovered.EmptyCell();
            }
        }
    }

    private bool CheckCanPlaceTowerOnCell()
    {
        TowerBaseParent towerSpawned;

        currentTowerPrefab.TryGetComponent<TowerBaseParent>(out towerSpawned);

        if (currentCellHovered.ReturnCellTypeColor() != CellTypeColor.Neutral &&
            currentCellHovered.ReturnCellTypeColor() != towerSpawned.ReturnCellColorType())
        {
            return false;
        }

        return true;
    }

    private void InstantiateTower()
    {
        Vector3 cellpos = currentCellHovered.transform.position;

        GameObject go = Instantiate(currentTowerPrefab, cellpos, Quaternion.identity);

        Vector3 fixedPos = new Vector3(cellpos.x, currentTowerPrefab.transform.position.y, cellpos.z); // temp here
        go.transform.position = fixedPos;

        TowerBaseParent towerSpawned;
        go.TryGetComponent<TowerBaseParent>(out towerSpawned);

        if (towerSpawned)
        {
            towerSpawned.InitTowerData(currentCellHovered.ReturnPositionInGridArray());
        }

        currentCellHovered.SetAsOccupied(go);
    }

    private GridCell MouseOverGridCell()
    {
        currentCellHovered = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        positionOfMouse = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, gridCellLayer))
        {
            hitInfo.transform.TryGetComponent<GridCell>(out currentCellHovered);
        }

        return currentCellHovered;
    }
}
