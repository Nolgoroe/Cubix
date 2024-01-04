using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGridCell : MonoBehaviour
{

    [Header ("Placement Data")]
    [SerializeField] private Vector2Int positionXYInGridArray;

    [Header("Live Gameplay Data")]
    [SerializeField] private GameObject gameObjectOnCell;
    [SerializeField] private bool isOccupied;
    [SerializeField] private TypeOfCell cellType;

    [Header("Generation Data")]
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private GameObject spawnedWaypoint;
    [SerializeField] private MeshRenderer renderer;

    private Material cellMat;

    private void OnValidate()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();

        cellMat = renderer.materials[0];
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

    public void DisplayAsWaypoint(bool isWaypoint)
    {
        if(isWaypoint)
        {
            if (spawnedWaypoint) return;
            spawnedWaypoint = Instantiate(waypointPrefab, transform);
            spawnedWaypoint.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
        else
        {
            if(spawnedWaypoint)
            {
                Destroy(spawnedWaypoint);
            }
        }
    }

    public void CopyOtherGridCell(ToolGridCell otherCell)
    {
        positionXYInGridArray = otherCell.positionXYInGridArray;
        gameObjectOnCell = otherCell.gameObjectOnCell;
        //isOccupied = otherCell.isOccupied; for now this is set initially in prefabs.
    }

    public void ChangeMat(Material newMat)
    {
        renderer.material = newMat;
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

    public Color ReturnCellColor()
    {
        return cellMat.color;
    }
    public TypeOfCell ReturnTypeOfCell()
    {
        return cellType;
    }

    #endregion

}