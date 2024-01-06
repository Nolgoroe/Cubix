using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGridCell : MonoBehaviour
{

    [Header ("Placement Data")]
    [SerializeField] private Vector2Int positionXYInGridArray;

    [Header("Live Gameplay Data")]
    [SerializeField] private PlacedObject placedObject;
    [SerializeField] private bool isOccupied;
    [SerializeField] private TypeOfCell cellType;

    [Header("Generation Data")]
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private GameObject spawnedWaypoint;
    [SerializeField] private MeshRenderer renderer;

    [SerializeField] private Material cellMat;

    private void OnValidate()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();

        cellMat = renderer.materials[0];
    }

    #region Public Actions
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
            spawnedWaypoint = Instantiate(waypointPrefab, ToolReferencerObject.Instance.toolGameGrid.ReturnWaypointsParent());
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
        placedObject = otherCell.placedObject;
        //isOccupied = otherCell.isOccupied; for now this is set initially in prefabs.
    }

    public void ChangeMat(Material newMat)
    {
        cellMat = new Material(newMat);

        renderer.material = cellMat;
    }

    public void PopulateGridCell(PlacedObject toPlaceOnCell)
    {
        placedObject = toPlaceOnCell;
        isOccupied = true;
    }
    public void EmptyGridCell()
    {
        isOccupied = false;
        placedObject = null;

        ChangeCellColor(Color.white);

    }
    #endregion

    #region Returned Data
    public Vector2Int ReturnPosInGridArray()
    {
        return positionXYInGridArray;
    }
    public PlacedObject ReturnPlacedObject()
    {
        return placedObject;
    }
    public bool ReturnIsOccupied()
    {
        return isOccupied;
    }

    public Color ReturnCellColor()
    {
        return cellMat.color;
    }
    public Transform ReturnSpawnedWaypoint()
    {
        if(spawnedWaypoint)
        {
            return spawnedWaypoint.transform;
        }

        return null;
    }
    public TypeOfCell ReturnTypeOfCell()
    {
        return cellType;
    }

    #endregion

}
