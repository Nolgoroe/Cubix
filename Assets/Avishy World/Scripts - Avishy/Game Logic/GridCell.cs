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
    [SerializeField] private GameObject objectOnCell;


    private Renderer rend;

    private void OnValidate()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();

        if (outline == null)
            outline = GetComponent<Outline>();
    }
    protected virtual void Start()
    {
        startColor = rend.material.color;
    }


    public void OnMouseHover(bool isHoveredOn)
    {
        if (outline)
            outline.enabled = isHoveredOn ? true : false;

        if(outline && isHoveredOn)
        {
            outline.SetOutlineMode(Outline.Mode.OutlineAll);
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

    public void SetAsOccupied(GameObject objectToPlace)
    {
        objectOnCell = objectToPlace;
        isOccupied = true;

        occupiedByTower = true;
    }

    public void EmptyCell()
    {
        if(objectOnCell)
        {
            Destroy(objectOnCell);
        }

        isOccupied = false;

        
        if(occupiedByTower)
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
}
