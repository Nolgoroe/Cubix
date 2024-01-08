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

    [Header("Live Data")]
    [SerializeField] private bool isOccupied;
    [SerializeField] private bool occupiedByTower;
    [SerializeField] private GameObject objectOnCell;


    private Renderer rend;

    protected virtual void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }


    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }









    public Vector2Int ReturnPositionInGridArray()
    {
        return positionXYInGridArray;
    }

    public TypeOfCell ReturnTypeOfCell()
    {
        return cellType;
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

        occupiedByTower = true; // temp here
    }
    public void EmptyCell()
    {
        if(objectOnCell)
        {
            Destroy(objectOnCell);
        }

        isOccupied = false;

        //temp here
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
    }
}
