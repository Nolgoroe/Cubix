using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Placement Data")]
    [SerializeField] protected Vector2Int positionXYInGridArray;
    [SerializeField] private TypeOfCell cellType;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color startColor;
    [SerializeField] private SpriteRenderer slotTypeSpriteRenderer;
    [SerializeField] private CellTypeColor cellTypeColor;
    [SerializeField] private Outline outline;

    [Header("Live Data")]
    [SerializeField] private bool isOccupied;
    [SerializeField] private bool occupiedByTower;
    [SerializeField] private bool nextToPath;
    [SerializeField] private TowerBaseParent towerOnCell;


    [SerializeField] private MeshRenderer rend;

    private void OnValidate()
    {
        if (rend == null)
            rend = GetComponent<MeshRenderer>();

        if (outline == null)
            outline = GetComponent<Outline>();
    }
    protected virtual void Start()
    {
        if (rend == null)
            rend = GetComponent<MeshRenderer>();

        if (outline == null)
            outline = GetComponent<Outline>();

        startColor = rend.material.color;


        Vector2Int checkDown = new Vector2Int(positionXYInGridArray.x, positionXYInGridArray.y - 1);
        Vector2Int checkUp = new Vector2Int(positionXYInGridArray.x, positionXYInGridArray.y + 1);
        Vector2Int checkLeft = new Vector2Int(positionXYInGridArray.x - 1, positionXYInGridArray.y);
        Vector2Int checkRight = new Vector2Int(positionXYInGridArray.x + 1, positionXYInGridArray.y);

        Vector2Int[] directions = new Vector2Int[] { checkDown, checkUp, checkLeft, checkRight };

        foreach (Vector2Int pos in directions)
        {
            if (pos.x > -1 && pos.x < GridManager.Instance.ReturnWidthHeight().x
                && pos.y > -1 && pos.y < GridManager.Instance.ReturnWidthHeight().y)
            {
                GridCell cell = GridManager.Instance.ReturnCellAtVector(pos);
                if (cell.ReturnTypeOfCell() == TypeOfCell.enemyPath)
                {
                    nextToPath = true;
                    return;
                }
            }
        }
    }


    public void SetOnMouseHover(bool isHoveredOn)
    {
        if (outline)
            outline.enabled = isHoveredOn ? true : false;

        if(isHoveredOn)
        {
            if (outline)
            {
                outline.SetOutlineMode(Outline.Mode.OutlineAll);
            }

            if (occupiedByTower)
            {
                UIManager.Instance.DisplayTowerBuffData(true, towerOnCell);
            }
        }
        else
        {
            if (occupiedByTower)
            {
                UIManager.Instance.DisplayTowerBuffData(false, towerOnCell);
            }
        }

        if(towerOnCell)
        {
            towerOnCell.OnHoverOverOccupyingCell(isHoveredOn);
        }
    }

    public Vector2Int ReturnPositionInGridArray()
    {
        return positionXYInGridArray;
    }

    public TypeOfCell ReturnTypeOfCell()
    {
        return cellType;
    }
    public CellTypeColor ReturnCellTypeColor()
    {
        return cellTypeColor;
    }
    public bool ReturnIsOccipied()
    {
        return isOccupied;
    }
    public bool ReturnIsOccipiedByTower()
    {
        return occupiedByTower;
    }
    public TowerBaseParent ReturnTowerOnCell()
    {
        return towerOnCell;
    }

    public void SetAsOccupied(TowerBaseParent towerToPlace)
    {
        towerOnCell = towerToPlace;
        isOccupied = true;

        occupiedByTower = true; //temp
    }

    public void EmptyCell()
    {
        if(towerOnCell)
        {
            Destroy(towerOnCell);
        }

        isOccupied = false;

        
        if(occupiedByTower) //temp
        {
            occupiedByTower = false;
        }
    }

    public virtual void CopyDataFromToolCell(ToolGridCell toolGridCell)
    {
        positionXYInGridArray = toolGridCell.ReturnPosInGridArray();
        isOccupied = toolGridCell.ReturnIsOccupied();
        cellType = toolGridCell.ReturnTypeOfCell();
        cellTypeColor = toolGridCell.ReturnCellTypeColor();


        //rend.material.color = Color.white;
        outline = GetComponent<Outline>(); //temp
        if(outline)
        {
            outline.enabled = false;
            outline.SetOutlineMode(Outline.Mode.OutlineAll);
        }

        slotTypeSpriteRenderer = toolGridCell.ReturnSlotTypeSpriteRenderer();

        if (slotTypeSpriteRenderer == null) return;

        slotTypeSpriteRenderer.color = toolGridCell.ReturnTypeOfCellColor();
        if(cellTypeColor == CellTypeColor.None)
        {
            //this means that the cell is not a cell that can accept a tower.. so it's occupied
            isOccupied = true;
            slotTypeSpriteRenderer.gameObject.SetActive(false);
        }


    }

    public void ResetCellOnStartTurn()
    {
        if (slotTypeSpriteRenderer == null) return;
        cellTypeColor = CellTypeColor.None;
        isOccupied = true;
        occupiedByTower = false;
        towerOnCell = null;
        GridManager.Instance.RemoveCellFromTowerBaseCells(this);
        slotTypeSpriteRenderer.gameObject.SetActive(false);
    }
    public bool ReturnNextToPath()
    {
        return nextToPath;
    }
}
