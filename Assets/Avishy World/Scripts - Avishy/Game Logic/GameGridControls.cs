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

    [Header("Live data")]
    [SerializeField] private GridCell currentCellHovered;
    [SerializeField] private Die currentDieDragging;
    [SerializeField] private List<Die> currentDieToLock;


    [Header("Rally Point")]
    [SerializeField] TowerBaseParent currentTower;

    [Header("Temp variables")]
    public bool rapidControls; //temp




    private Vector3 positionOfMouse;
    bool SettingRallyPoint = false;

    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        MouseOverGridCell();

        RallyPointControls();

        NormalControls();

        DieDraggingControls();
    }

    private void RallyPointControls()
    {
        if (!SettingRallyPoint) return;
        currentTower.OnHoverOverOccupyingCell(true);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameManager.Instance.ReturnMainCamera().transform.position.y);
            Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, gridCellLayer))
            {
                MeleeTowerParentScript meleeTower = currentTower as MeleeTowerParentScript;
                if (meleeTower)
                {
                    meleeTower.SetRallyPoint(currentCellHovered);
                }
            }
            else
            {
            }

            currentTower.OnHoverOverOccupyingCell(false);
            SettingRallyPoint = false;
        }
    }

    private void DieDraggingControls()
    {
        if (currentDieDragging)
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameManager.Instance.ReturnMainCamera().transform.position.y);
            Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, gridCellLayer))
            {
                Vector3 offset = new Vector3(0, 0.5f, 0);
                currentDieDragging.transform.position = hit.point + offset;
            }
            else
            {
                Vector3 worldPos = GameManager.Instance.ReturnMainCamera().ScreenToWorldPoint(screenPos);

                currentDieDragging.transform.position = worldPos;

            }
        }
    }

    private void NormalControls()
    {
        if (SettingRallyPoint) return;

        if (currentCellHovered && currentCellHovered.ReturnIsOccipiedByTower())
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentTower = currentCellHovered.ReturnTowerOnCell();
                SettingRallyPoint = true;
            }
        }
            
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

                    SetCurrentDieDragging(null);

                    SoundManager.Instance.ActivateSoundImmediate(Sounds.PlaceTower);
                }
                else
                {
                    //release die back to player zone
                    currentDieDragging.OnDragEndEvent?.Invoke();
                    SetCurrentDieDragging(null);

                }
            }
        }
    }

    private bool CheckCanPlaceTowerOnCell()
    {
        TowerBaseParent towerSpawned;

        currentTowerPrefab.TryGetComponent<TowerBaseParent>(out towerSpawned);

        if (currentCellHovered.ReturnCellTypeColor() == CellTypeColor.Neutral ||
            currentCellHovered.ReturnCellTypeColor() != towerSpawned.ReturnCellColorType())
        {
            return false;
        }

        return true;
    }


    private void InstantiateTower()
    {
        Vector3 cellpos = currentCellHovered.transform.position;
        Vector3 fixedPos = new Vector3(cellpos.x, cellpos.y + currentTowerPrefab.transform.position.y, cellpos.z);

        if(rapidControls)
        {
            if (currentDieDragging.ReturnCurrentTowerParent())
            {
                currentDieDragging.ReturnCurrentTowerParent().gameObject.SetActive(true);
                currentDieDragging.ReturnCurrentTowerParent().transform.position = fixedPos;
                currentCellHovered.SetAsOccupied(currentDieDragging.ReturnCurrentTowerParent());

                currentDieDragging.ReturnCurrentTowerParent().InitTowerData(currentCellHovered.ReturnPositionInGridArray(), currentDieDragging);

                currentDieDragging.OnPlaceEvent?.Invoke();

                SetCurrentDieDragging(null);
            }
            else
            {
                TowerSpawnAction(cellpos, fixedPos);
            }
        }
        else
        {
            TowerSpawnAction(cellpos, fixedPos);
        }
    }

    private void TowerSpawnAction(Vector3 cellpos, Vector3 fixedPos)
    {
        GameObject go = Instantiate(currentTowerPrefab, cellpos, Quaternion.identity);

        go.transform.position = fixedPos;

        TowerBaseParent towerSpawned;
        go.TryGetComponent<TowerBaseParent>(out towerSpawned);

        if (towerSpawned)
        {
            towerSpawned.InitTowerData(currentCellHovered.ReturnPositionInGridArray(), currentDieDragging);


            //End die drag and return it's values to be able to be rolled
            currentDieDragging.OnPlaceEvent?.Invoke();
            currentDieDragging.InitDie(towerSpawned);

            SetCurrentDieDragging(null);

            currentCellHovered.SetAsOccupied(towerSpawned);
        }
    }

    private GridCell MouseOverGridCell()
    {
        if (UIManager.menuOpened || EventSystem.current.IsPointerOverGameObject()) return null;

        if(currentCellHovered)
        {
            currentCellHovered.SetOnMouseHover(false);
        }

        GridCell currentCell = currentCellHovered;

        currentCellHovered = null;
        Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(Input.mousePosition);

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
