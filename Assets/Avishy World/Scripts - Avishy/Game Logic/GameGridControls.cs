using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameGridControls : MonoBehaviour
{
    public static GameGridControls Instance;

    [Header("Preset Data")]
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private GameObject currentTowerPrefab;

    [Header("Automated data")]
    [SerializeField] private GridCell currentCellHovered;
    [SerializeField] private Die currentDieDragging;

    [SerializeField] private List<Die> currentDieToLock;



    private Vector3 positionOfMouse;


    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        MouseOverGridCell();

        NormalControls();

        if(currentDieDragging)
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,  Camera.main.transform.position.y);
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, gridCellLayer))
            {
                Vector3 offset = new Vector3(0, 0.5f, 0);
                currentDieDragging.transform.position = hit.point + offset;
            }
            else
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                currentDieDragging.transform.position = worldPos;

            }
        } 
    }

    private void NormalControls()
    {

        if (currentDieDragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (currentCellHovered && !currentCellHovered.ReturnIsOccipied())
                {
                    InstantiateTower();

                    if (CheckCanPlaceTowerOnCell())
                    {
                        Player.Instance.RecieveRandomResource();
                    }

                    //currentDieDragging.OnDestroyDieEvent?.Invoke();

                    SetCurrentDieDragging(null);
                }
                else
                {
                    //release die back to player zone
                    currentDieDragging.OnDragEndEvent?.Invoke();
                    SetCurrentDieDragging(null);

                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentCellHovered && currentCellHovered.ReturnIsOccipied() && currentCellHovered.ReturnIsOccipiedByTower())
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

        Vector3 fixedPos = new Vector3(cellpos.x, currentTowerPrefab.transform.position.y, cellpos.z);
        go.transform.position = fixedPos;

        TowerBaseParent towerSpawned;
        go.TryGetComponent<TowerBaseParent>(out towerSpawned);

        if (towerSpawned)
        {
            towerSpawned.InitTowerData(currentCellHovered.ReturnPositionInGridArray(), currentDieDragging);
            

            //End die drag and return it's values to be able to be rolled
            currentDieDragging.OnPlaceEvent?.Invoke();
            SetCurrentDieDragging(null);

            currentCellHovered.SetAsOccupied(towerSpawned);
        }
    }

    private GridCell MouseOverGridCell()
    {
        if (UIManager.menuOpened) return null;

        if(currentCellHovered)
        {
            currentCellHovered.SetOnMouseHover(false);
        }

        GridCell currentCell = currentCellHovered;

        currentCellHovered = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        positionOfMouse = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, gridCellLayer))
        {
            hitInfo.transform.TryGetComponent<GridCell>(out currentCellHovered);
        }

        if(currentCellHovered)
        {
            currentCellHovered.SetOnMouseHover(true);
        }

        return currentCellHovered;
    }
    public GridCell ReturnCurrentCell()
    {        

        return currentCellHovered;
    }

    public void SetCurrentDieDragging(Die dieToDrag)
    {       

        if (dieToDrag)
        {
            currentDieDragging = dieToDrag;
            currentTowerPrefab = dieToDrag.ReturnTowerPrefab();
        }
        else
        {
            currentDieDragging = null;
        }
    }
}
