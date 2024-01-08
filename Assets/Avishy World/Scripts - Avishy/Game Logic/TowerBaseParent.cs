using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerBaseParent : MonoBehaviour
{
    [SerializeField] protected Vector2Int currentCellOnPos;

    public abstract void InitTowerData(Vector2Int positionOfCell);
}
