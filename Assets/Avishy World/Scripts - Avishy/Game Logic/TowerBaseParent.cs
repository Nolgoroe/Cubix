using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerBaseParent : MonoBehaviour
{
    [SerializeField] protected Vector2Int currentCellOnPos;
    [SerializeField] protected CellTypeColor requiredCellColorType;

    public abstract void InitTowerData(Vector2Int positionOfCell);

    public CellTypeColor ReturnCellColorType()
    {
        return requiredCellColorType;
    }
}
