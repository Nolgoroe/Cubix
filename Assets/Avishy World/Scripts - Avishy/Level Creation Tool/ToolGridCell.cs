using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGridCell : MonoBehaviour
{    public int GetRotationAngle()
    {
        switch (pathMeshSides)
        {
            case PathSides.up: return 0;
            case PathSides.left: return 90;
            case PathSides.down: return 0;
            case PathSides.right: return -90;
            case PathSides.leftUp: return 180;
            case PathSides.rightUp: return 90;
            case PathSides.leftDown: return -90;
            case PathSides.rightDown: return 0;
            case PathSides.leftRightUp: return 90;
            case PathSides.leftUpDown: return 180;
            case PathSides.rightUpDown: return 0;
            case PathSides.leftRightDown: return -90;
            case PathSides.downLeftUp: return -180;
            case PathSides.downRightUp: return 0;
            case PathSides.leftRightUpDown: return 0;
            case PathSides.None: return 0;
            default: return 0;
        }

    }
    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            case Dir.Down: return 0;
            case Dir.Up: return 180;
            case Dir.Left: return 90;
            case Dir.Right: return 270;
            default: return -1;
        }

    }

    [Header ("Placement Data")]
    [SerializeField] protected Vector2Int positionXYInGridArray;
    [SerializeField] protected PathSides pathMeshSides;

    [Header("Live Gameplay Data")]
    [SerializeField] private PlacedObject placedObject;
    [SerializeField] private bool isOccupied;
    [SerializeField] private TypeOfCell cellType;
    [SerializeField] private Dir currentDir = Dir.Down;
    [SerializeField] private CellTypeColor cellTypeColor;

    [Header("Generation Data")]
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private SpriteRenderer slotTypeSpriteRenderer;

    [Header("Automated Data")]
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private Material cellMat;
    [SerializeField] private GameObject spawnedWaypoint;

    private void OnValidate()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        cellTypeColor = CellTypeColor.None;

        renderer = GetComponent<MeshRenderer>();

        cellMat = renderer.materials[0];

        if (cellType != TypeOfCell.enemyPath) return;
        Quaternion RotationByDir = Quaternion.Euler(0, 0, GetRotationAngle());
        transform.localRotation = RotationByDir;
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
    public void ChangeCellTypeColor(CellTypeColor _cellTypeColor)
    {
        //slotTypeSpriteRenderer.gameObject.SetActive(false);
        cellTypeColor = _cellTypeColor;

        //slotTypeSpriteRenderer.gameObject.SetActive(true);

        Color color = ToolReferencerObject.Instance.levelCreationToolSO.ReturnColorByCellTypeColor(cellTypeColor);

        slotTypeSpriteRenderer.color = color;
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


        cellMat = renderer.materials[0];

        cellMat.color = otherCell.cellMat.color;
        //isOccupied = otherCell.isOccupied; for now this is set initially in prefabs.
    }

    public void ChangeMat(Material newMat)
    {
        cellMat = new Material(newMat);

        renderer.material = cellMat;
    }
    public void PermaChangeMat(Material newMat)
    {
        renderer.material = newMat;

        cellMat = newMat;
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
    public CellTypeColor ReturnCellTypeColor()
    {
        return cellTypeColor;
    }
    public Color ReturnTypeOfCellColor()
    {
        if (!slotTypeSpriteRenderer) return Color.white;
        return slotTypeSpriteRenderer.color;
    }
    public SpriteRenderer ReturnSlotTypeSpriteRenderer()
    {
        if (!slotTypeSpriteRenderer) return null;
        return slotTypeSpriteRenderer;
    }

    #endregion

}
