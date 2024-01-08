using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridControls : MonoBehaviour
{
    [Header("Preset Data")]
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private GameObject towerPrefab;// temp here - think of a place to hold all the towers available and choose.

    [Header("Automated data")]
    [SerializeField] private GridCell currentCellHovered;




    private Vector3 positionOfMouse;

    private void Update()
    {
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
                //this is where we will run checks by towerType (range/Melee) to see if we can place that tower there
                // or if we have specific slots, we don't need this step.

                Vector3 cellpos = currentCellHovered.transform.position;

                Vector3 pos = new Vector3(cellpos.x, cellpos.y + 0.5f, cellpos.z); // temp here

                GameObject go = Instantiate(towerPrefab, pos, Quaternion.identity);

                TowerBaseParent towerSpawned;
                go.TryGetComponent<TowerBaseParent>(out towerSpawned);

                if(towerSpawned)
                {
                    towerSpawned.InitTowerData(currentCellHovered.ReturnPositionInGridArray());
                }

                currentCellHovered.SetAsOccupied(go);
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
