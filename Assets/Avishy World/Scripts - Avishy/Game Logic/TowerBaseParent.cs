using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerBaseParent : MonoBehaviour
{
    [SerializeField] protected Vector2Int currentCellOnPos;
    [SerializeField] protected CellTypeColor requiredCellColorType;
    [SerializeField] protected Die towerDie;         //new avishy
    [SerializeField] protected Transform rangeIndicator;


    public abstract void InitTowerData(Vector2Int positionOfCell, Die connectedDie);        //new avishy
    public abstract void RecieveBuffAfterRoll(Die die);

    private void Start()
    {
        if(towerDie)
        {
            towerDie.OnRollEndEvent.AddListener(RecieveBuffAfterRoll);
        }
    }

    public CellTypeColor ReturnCellColorType()
    {
        return requiredCellColorType;
    }

}
