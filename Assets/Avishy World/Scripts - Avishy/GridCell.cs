using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    [Header ("Generated Data")]

    [SerializeField] private Vector2Int positionXYInGridArray;
    [Header("Live Gameplay Data")]
    [SerializeField] private GameObject gameObjectOnCell;
    [SerializeField] private bool isOccupied;

    private Material cellMat;

    private void Start()
    {
        cellMat = GetComponent<MeshRenderer>().materials[0];
    }

    #region Actions
    public void SetXYInGrid(int x, int y)
    {
        positionXYInGridArray.x = x;
        positionXYInGridArray.y = y;
    }
    public void ChangeCellColor(Color toChange)
    {
        cellMat.color = toChange;
    }

    public void CopyOtherGridCell(GridCell otherCell)
    {
        positionXYInGridArray = otherCell.positionXYInGridArray;
        gameObjectOnCell = otherCell.gameObjectOnCell;
        //isOccupied = otherCell.isOccupied; for now this is set initially in prefabs.
    }
    #endregion

    #region Returned Data
    public Vector2Int ReturnPosInGridArray()
    {
        return positionXYInGridArray;
    }
    public GameObject ReturnGameObjectOnCell()
    {
        return gameObjectOnCell;
    }
    public bool ReturnIsOccupied()
    {
        return isOccupied;
    }

    public Color returnCellColor()
    {
        return cellMat.color;
    }

    #endregion
}
