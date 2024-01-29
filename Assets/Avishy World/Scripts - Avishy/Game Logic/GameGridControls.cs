using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameGridControls : MonoBehaviour
{
    public static GameGridControls Instance;

    [Header("Preset Data")]
    [SerializeField] private LayerMask cellDetectionLayer;
    [SerializeField] private GameObject currentTowerPrefab;

    [Header("Live data")]
    [SerializeField] private GridCell currentCellHovered;
    [SerializeField] private Die currentDieDragging;
    [SerializeField] private List<Die> currentDieToLock;


    [Header("Tower Stamina System")]
    [SerializeField] private float timeToStartMoving = 1;
    [SerializeField] private float currentTimeToStartMoving = 1;
    [SerializeField] private bool isMovingTower;
    [SerializeField] private Vector3 originalTowerPos;
    [SerializeField] private GridCell originalCell;
    [SerializeField] TowerBaseParent currentTowerMoving;

    [Header("Rally Point")]
    [SerializeField] TowerBaseParent currentTowerSelected;

    [Header("Spell Controls")]
    [SerializeField] SpellParent currentSpell;
    [SerializeField] bool isUsingSpell;

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

        if (isMovingTower)
        {
            MovingTowerControls();
            return;
        }
        MouseOverSpellCell();
        if (isUsingSpell)
        {
            UsingSpellControls();
            return;
        }


        RallyPointControls();

        DieDraggingControls();


        if (SettingRallyPoint || isUsingSpell) return;
        NormalControls();
    }

    private void MovingTowerControls()
    {
        Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameManager.Instance.ReturnMainCamera().transform.position.y);
        Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, cellDetectionLayer))
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);
            currentTowerMoving.transform.position = hit.point + offset;

            if (Input.GetMouseButtonUp(0))
            {
                if(currentCellHovered && !currentCellHovered.ReturnIsOccipied() && currentCellHovered !=originalCell)
                {
                    currentCellHovered.SetAsOccupied(currentTowerMoving);
                    isMovingTower = false;
                    currentTowerMoving.SetAsDisabled(true);
                    currentTowerMoving.SetAsBeingDragged(false);
                    currentTowerMoving.ManualSetTowerOnCell(currentCellHovered.ReturnPositionInGridArray());

                    Vector3 cellpos = currentCellHovered.transform.position;
                    Vector3 fixedPos = new Vector3(cellpos.x, cellpos.y + currentTowerPrefab.transform.position.y, cellpos.z);

                    currentTowerMoving.transform.position = fixedPos;
                    return;
                }
                else
                {
                    ReturnTowerToOriginalPos();
                    return;
                }
            }

        }
        else
        {
            Vector3 worldPos = GameManager.Instance.ReturnMainCamera().ScreenToWorldPoint(screenPos);

            currentTowerMoving.transform.position = worldPos;

        }


        if(Input.GetMouseButtonUp(0))
        {
            ReturnTowerToOriginalPos();
            return;
        }
    }

    private void ReturnTowerToOriginalPos()
    {
        //move tower back to original pos
        currentTowerMoving.SetAsBeingDragged(false);
        originalCell.SetAsOccupied(currentTowerMoving);
        UIManager.Instance.DisplayTowerBuffData(false, null);
        currentTowerMoving.transform.position = originalTowerPos;
        isMovingTower = false;
    }

    private void RallyPointControls()
    {
        if (!SettingRallyPoint) return;
        currentTowerSelected.OnHoverOverOccupyingCell(true);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameManager.Instance.ReturnMainCamera().transform.position.y);
            Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, cellDetectionLayer))
            {
                MeleeTowerParentScript meleeTower = currentTowerSelected as MeleeTowerParentScript;
                if (meleeTower)
                {
                    meleeTower.SetRallyPoint(currentCellHovered);
                }
            }

            currentTowerSelected.OnHoverOverOccupyingCell(false);
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

            if (Physics.Raycast(ray, out hit, 1000, cellDetectionLayer))
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

        if (currentCellHovered && currentCellHovered.ReturnIsOccipiedByTower())
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentTowerSelected = currentCellHovered.ReturnTowerOnCell();
                SettingRallyPoint = true;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                currentTimeToStartMoving = timeToStartMoving;
            }

            if (Input.GetMouseButton(0))
            {
                if(currentTimeToStartMoving <= 0)
                {
                    currentTimeToStartMoving = timeToStartMoving;
                    isMovingTower = true;
                    currentTowerMoving = currentCellHovered.ReturnTowerOnCell();
                    currentTowerMoving.SetAsBeingDragged(true);
                    UIManager.Instance.DisplayTowerBuffData(false, null);

                    originalCell = currentCellHovered;
                    currentCellHovered.ResetCell();

                    originalTowerPos = currentTowerMoving.transform.position;

                    return;
                }

                currentTimeToStartMoving -= Time.deltaTime * GameManager.gameSpeed;
                //we are holding down the mousebutton on a base, meaning we want to drag it.
            }
        }
            
        if (currentDieDragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (currentCellHovered && !currentCellHovered.ReturnIsOccipied() && GameManager.playerTurn)
                {
                    InstantiateTower();

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
    private void UsingSpellControls()
    {            
        if (currentDieDragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if(currentSpell)
                {
                    /// call activate spell here
                    if(currentSpell.UseSpell(currentDieDragging))
                    {
                        DiceManager.Instance.RemoveDiceToResources(currentDieDragging);

                        currentSpell = null;
                        isUsingSpell = false;
                        currentDieDragging = null;
                    }
                }
                else
                {
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


            currentCellHovered.SetAsOccupied(towerSpawned);

            if(CheckCanPlaceTowerOnCell())
            {
                towerSpawned.RecieveRandomBuff(currentDieDragging);
            }

            SetCurrentDieDragging(null);

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

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cellDetectionLayer))
        {
            hitInfo.transform.TryGetComponent<GridCell>(out currentCellHovered);
        }

        if(currentCellHovered)
        {
            currentCellHovered.SetOnMouseHover(true);
        }

        return currentCellHovered;
    }
    private bool MouseOverSpellCell()
    {
        if (UIManager.menuOpened || 
            EventSystem.current.IsPointerOverGameObject() ||
            GameManager.playerTurn) return false;

        currentSpell = null;
        isUsingSpell = false;
        Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(Input.mousePosition);

        positionOfMouse = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cellDetectionLayer))
        {
            hitInfo.transform.TryGetComponent<SpellParent>(out currentSpell);
        
            if(currentDieDragging && currentSpell)
            {
                currentDieDragging.ToggleRangeIndicator(false);

                if (currentSpell.SnapToHolder(currentDieDragging))
                {
                    isUsingSpell = true;
                    return true;
                }

            }
        }

        return false;
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
