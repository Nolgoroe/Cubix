using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TypeOfCell
{
    enemyPath,
    enemySpawner,
    Obstacle,
    PlayerBase,
    None,
    Waypoints
}

public class LevelCreationToolControls : MonoBehaviour
{
    [Header("Needed References")]
    [SerializeField] private GameObject toolGameGridPrefab;
    [SerializeField] private BuildingGhost buildingGhost;

    [Header("Cell Detection")]
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private ToolGridCell currentCellHovered;
    [SerializeField] private ToolGridCell currentCellSelected;
    [SerializeField] private ToolEnemySpawnerCell currentSpawnerSelected;

    [Header("Draw Data")]
    [SerializeField] private TypeOfCell currentTypeOfCellSelected;
    [SerializeField] private bool isDrawingWaypoints;
    [SerializeField] private bool isInBuildMode;

    [Header("Build Mode Data")]
    [SerializeField] private int currentBuildingIndex;
    [SerializeField] private List<BuildingsSO> buildingsList;
    [SerializeField] private List<GameObject> rotatingBuildingList;
    [SerializeField] private BuildingsSO.Dir currentDir = BuildingsSO.Dir.Down;
    [SerializeField] private Transform rotatingBuildingParent;

    private BuildingsSO CurrentBuildingSelected;

    private Vector3 positionOfMouse;

    private void Start()
    {
        rotatingBuildingList = new List<GameObject>();

        GameObject go = Instantiate(toolGameGridPrefab);
        go.TryGetComponent<ToolGameGrid>(out ToolReferencerObject.Instance.toolGameGrid);


        foreach (BuildingsSO building in buildingsList)
        {
            GameObject rotatingBuilding = Instantiate(building.visualPrefab, rotatingBuildingParent);
            rotatingBuilding.gameObject.SetActive(false);
            rotatingBuildingList.Add(rotatingBuilding);
        }


        CurrentBuildingSelected = buildingsList[0];
        buildingGhost.SetCurrentBuilding(CurrentBuildingSelected);
        rotatingBuildingList[0].SetActive(true);
    }
    private void Update()
    {
        //Create new grid on press X
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (ToolReferencerObject.Instance.toolGameGrid == null)
            {
                GameObject go = Instantiate(toolGameGridPrefab);
                go.TryGetComponent<ToolGameGrid>(out ToolReferencerObject.Instance.toolGameGrid);
            }

            if (ToolReferencerObject.Instance.toolGameGrid)
            {
                ToolReferencerObject.Instance.toolGameGrid.InitNewGrid();
                ToolReferencerObject.Instance.toolUI.ResetLevelName();
            }
            else
            {
                Debug.LogError("Could not create grid");
            }
        }

        if (EventSystem.current.IsPointerOverGameObject() || CameraControls.isDuringCamMovement) return; // this checks if the mouse pointer is over ui object
        Debug.Log("Check");
        //Detect cells i'm hovering above
        MouseOverGridCell();

        //Switch controls by modes
        if (currentCellHovered)
        {
            Vector3 cellPos = currentCellHovered.transform.position;
            cellIndicator.transform.position = new Vector3(cellPos.x, cellPos.y + 0.75f, cellPos.z);
            cellIndicator.transform.rotation = currentCellHovered.transform.rotation;

            if (isDrawingWaypoints)
            {
                DrawingWaypoinyControls();
            }
            else if(isInBuildMode)
            {
                BuildModeControls();
            }
            else
            {
                NormalControls();
            }

            //Detect middle mouse to "Select" a cell
            // currently only really valid for enemy spawner as that is how we move to draw waypoint mode
            if (Input.GetMouseButton(2))
            {
                currentCellSelected = currentCellHovered;

                MiddileClickOnCell(currentCellSelected);
            }
        }
    }

    private void NormalControls()
    {
        if (Input.GetMouseButton(0))
        {
            currentCellHovered.ChangeCellColor(ToolReferencerObject.Instance.levelCreationToolSO.ConvertTypeToColor(currentTypeOfCellSelected));
        }
        if (Input.GetMouseButtonDown(1))
        {
            currentCellHovered.ChangeCellColor(ToolReferencerObject.Instance.levelCreationToolSO.ConvertTypeToColor(TypeOfCell.None));
        }
    }
    private void BuildModeControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Transform buildingParent = ToolReferencerObject.Instance.toolGameGrid.ReturnBuildingsParent();

            if (CurrentBuildingSelected.snapToGrid)
            {
                List<Vector2Int> gridPosList = CurrentBuildingSelected.GetGridPositionList(currentCellHovered.ReturnPosInGridArray(), currentDir);
                ToolGridCell[,] temp2DArray = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray();

                Vector2 gridWidthHeight = ToolReferencerObject.Instance.toolGameGrid.ReturnGridWidthAndHeight();

                foreach (Vector2Int gridPos in gridPosList)
                {

                    if (gridPos.x >= gridWidthHeight.x || gridPos.x < 0 ||
                        gridPos.y >= gridWidthHeight.y || gridPos.y < 0)
                    {
                        return;
                    }
                    if (temp2DArray[gridPos.x, gridPos.y].ReturnIsOccupied())
                    {
                        return;
                    }
                }


                Vector2 rotationOffset = CurrentBuildingSelected.GetRotationOffset(currentDir); // gets the amount on X and Y that the object needs to move according to it's dircetion and size
                Vector3 poisitonToAddByRotationOffset = new Vector3(rotationOffset.x, 0, rotationOffset.y); //we only move objects on X and Z.
                Vector3 correctionToCellPivot = new Vector3(-0.5f, 0, -0.5f); // we want to force the pivot of the building to be on the EDGE of a cell. this fixes rotations into slots.

                // the position of the building is the position of the cell + the position in the prefab
                // + the position fixed by the corrected pivot + the position we need to add by the direction and rotation we are facing.
                // we add the position by rotation since our pivot is at the edge of the building and cell.
                // so for example, if we are looking left, when we add 90 degrees, our left edge of the building would be at the bottom right edge of cell we clicked on.
                // so we'll need to move it, by it's width, a few steps towards the Z.
                Vector3 buildingPos = correctionToCellPivot +
                    currentCellHovered.transform.position +
                    CurrentBuildingSelected.buildingPrefab.transform.position +
                    poisitonToAddByRotationOffset;

                PlacedObject placedObject = PlacedObject.Create(buildingPos, currentCellHovered.ReturnPosInGridArray(), currentDir, CurrentBuildingSelected, buildingParent);

                foreach (Vector2Int gridPos in gridPosList)
                {
                    temp2DArray[gridPos.x, gridPos.y].PopulateGridCell(placedObject);
                }
            }
            else
            {
                PlacedObject placedObject = PlacedObject.Create(MouseOverWorldNormal(), currentDir, CurrentBuildingSelected, buildingParent);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            PlacedObject placedObject = currentCellHovered.ReturnPlacedObject();
            if (placedObject)
            {
                placedObject.DestroySelf();

                List<Vector2Int> gridPosList = placedObject.GetGridPositionsList();// this will return all of the cells that are part of this building by it's width and height

                foreach (Vector2Int gridPos in gridPosList)
                {
                    ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray()[gridPos.x, gridPos.y].EmptyGridCell();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            currentDir = BuildingsSO.GetNextDir(currentDir);
            Debug.Log(currentDir);
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            rotatingBuildingList[currentBuildingIndex].SetActive(false);

            currentBuildingIndex++;

            if(currentBuildingIndex > buildingsList.Count - 1)
            {
                currentBuildingIndex = 0;
            }

            CurrentBuildingSelected = buildingsList[currentBuildingIndex];
            buildingGhost.SetCurrentBuilding(CurrentBuildingSelected);

            rotatingBuildingList[currentBuildingIndex].SetActive(true);
        }
    }
    private void DrawingWaypoinyControls()
    {
        if (currentSpawnerSelected == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            currentSpawnerSelected.AddToEnemyPath(currentCellHovered);
        }
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(currentSpawnerSelected.RemoveFromEnemyPath(currentCellHovered));
        }
    }
    private void MiddileClickOnCell(ToolGridCell cell)
    {
        currentSpawnerSelected = null; //only set spawner when middile clicking a spawner

        switch (cell.ReturnTypeOfCell())
        {
            case TypeOfCell.enemyPath:
                isDrawingWaypoints = false;
                currentTypeOfCellSelected = TypeOfCell.enemyPath;

                break;
            case TypeOfCell.enemySpawner:
                currentSpawnerSelected = cell as ToolEnemySpawnerCell;
                currentTypeOfCellSelected = TypeOfCell.Waypoints;
                isDrawingWaypoints = true;

                ///start new path here.
                break;
            case TypeOfCell.Obstacle:
                isDrawingWaypoints = false;
                currentTypeOfCellSelected = TypeOfCell.Obstacle;

                break;
            case TypeOfCell.PlayerBase:
                isDrawingWaypoints = false;
                currentTypeOfCellSelected = TypeOfCell.PlayerBase;
                break;
            case TypeOfCell.None:
                isDrawingWaypoints = false;
                currentTypeOfCellSelected = TypeOfCell.None;
                break;
            default:
                break;
        }

        ToolReferencerObject.Instance.toolUI.SetDropdownToValue((int)currentTypeOfCellSelected);
    }

    private ToolGridCell MouseOverGridCell()
    {
        currentCellHovered = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        positionOfMouse = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, gridCellLayer))
        {
            hitInfo.transform.TryGetComponent<ToolGridCell>(out currentCellHovered);
        }

        return currentCellHovered;
    }
    public Vector3 MouseOverWorldNormal()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, gridCellLayer))
        {
            Debug.Log(hitInfo.point);
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    public void CallGenerateLevel()
    {
        ToolReferencerObject.Instance.toolGameGrid.GenerateTheLevel();
    }
    public void SetCurrentTypeOfCellSelected(TypeOfCell typeOfCell)
    {
        currentTypeOfCellSelected = typeOfCell;

        switch (typeOfCell)
        {
            case TypeOfCell.enemyPath:
                isDrawingWaypoints = false;
                break;
            case TypeOfCell.enemySpawner:
                isDrawingWaypoints = false;
                break;
            case TypeOfCell.Obstacle:
                isDrawingWaypoints = false;
                break;
            case TypeOfCell.PlayerBase:
                isDrawingWaypoints = false;
                break;
            case TypeOfCell.None:
                isDrawingWaypoints = false;
                break;
            case TypeOfCell.Waypoints:
                isDrawingWaypoints = true;
                break;
            default:
                break;
        }
        ToolReferencerObject.Instance.toolUI.SetDropdownToValue((int)currentTypeOfCellSelected);
    }
    public void CallSavePathCreated()
    {
        if (currentSpawnerSelected)
        {
            StartCoroutine(currentSpawnerSelected.SaveCreatedPath());
        }
    }
    public void CallDisplaySpecificPath(bool show, int index)
    {
        if (currentSpawnerSelected)
        {
            StartCoroutine(currentSpawnerSelected.DisplaySpecificPath(show, index));
        }
    }
    public void CallDeleteSpecificPath(int index)
    {
        if (currentSpawnerSelected)
        {
            StartCoroutine(currentSpawnerSelected.DeleteSpecificPath(index));
        }
    }
    public void SwitchToAndFromBuildMode()
    {
        isInBuildMode = !isInBuildMode;
        buildingGhost.ToggleGhost();
        SetCurrentTypeOfCellSelected(TypeOfCell.None);
        ToolReferencerObject.Instance.toolUI.ToggleBuildModeToggle(isInBuildMode);
    }

    public ToolGridCell ReturnCurrentCellHovered()
    {
        return currentCellHovered;
    }
    public BuildingsSO.Dir ReturnCurrentDir()
    {
        return currentDir;
    }

}
