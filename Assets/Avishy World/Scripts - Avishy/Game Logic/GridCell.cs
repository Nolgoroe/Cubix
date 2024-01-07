using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Placement Data")]
    [SerializeField] protected Vector2Int positionXYInGridArray;

    [SerializeField] private bool isOccupied;
    [SerializeField] private TypeOfCell cellType;

























    public Vector2Int ReturnPositionInGridArray()
    {
        return positionXYInGridArray;
    }

    public virtual void CopyDataFromToolCell(ToolGridCell toolGridCell)
    {
        positionXYInGridArray = toolGridCell.ReturnPosInGridArray();
        isOccupied = toolGridCell.ReturnIsOccupied();
        cellType = toolGridCell.ReturnTypeOfCell();
    }
}
