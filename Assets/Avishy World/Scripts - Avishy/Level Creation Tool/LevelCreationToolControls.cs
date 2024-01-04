using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] ToolReferencerObject referenceObject;

    [Header("Cell Detection")]
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private ToolGridCell currentCellHovered;
    [SerializeField] private ToolGridCell currentCellSelected;
    [SerializeField] private ToolEnemySpawnerCell currentSpawnerSelected;

    [Header("Draw Data")]
    [SerializeField] private TypeOfCell currentTypeOfCellSelected;
    [SerializeField] private bool isDrawingWaypoints;


    private Vector3 positionOfMouse;


    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.X))
        {
            if (referenceObject.gameGrid == null)
            {
                GameObject go = Instantiate(toolGameGridPrefab);
                go.TryGetComponent<ToolGameGrid>(out referenceObject.gameGrid);
            }

            if(referenceObject.gameGrid)
            {
                referenceObject.gameGrid.InitNewGrid();
            }
            else
            {
                Debug.LogError("Could not create grid");
            }
        }


        MouseOverGridCell();

        if (currentCellHovered)
        {
            Vector3 cellPos = currentCellHovered.transform.position;
            cellIndicator.transform.position = new Vector3(cellPos.x, cellPos.y + 0.75f, cellPos.z);

            if(isDrawingWaypoints)
            {
                DrawingWaypoinyControls();
            }
            else
            {
                NormalControls();
            }

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
            currentCellHovered.ChangeCellColor(referenceObject.levelCreationToolSO.ConvertTypeToColor(currentTypeOfCellSelected));
        }
        if (Input.GetMouseButton(1))
        {
            currentCellHovered.ChangeCellColor(referenceObject.levelCreationToolSO.ConvertTypeToColor(TypeOfCell.None));
        }
    }
    private void DrawingWaypoinyControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentCellHovered.DisplayAsWaypoint(true);
            currentSpawnerSelected.AddToEnemyPath(currentCellHovered);
        }
        if (Input.GetMouseButtonDown(1))
        {
            currentCellHovered.DisplayAsWaypoint(false);
            currentSpawnerSelected.RemoveFromEnemyPath(currentCellHovered);
        }
    }
    private void MiddileClickOnCell(ToolGridCell cell)
    {
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

        referenceObject.toolUI.SetDropdownToValue((int)currentTypeOfCellSelected);
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

    [ContextMenu("Generate The Level")]
    public void GenerateTheLevel()
    {
        referenceObject.gameGrid.ClearDataBeforeLevelGeneration();

        foreach (ToolGridCell cell in referenceObject.gameGrid.ReturnCellsArray())
        {
            GameObject toSpawn = referenceObject.levelCreationToolSO.SpawnPrefabByColor(cell.ReturnCellColor());

            if (toSpawn == null) continue;

            GameObject newObject = Instantiate(toSpawn, cell.transform.parent);

            newObject.TryGetComponent<ToolGridCell>(out ToolGridCell createdCell);
            if(createdCell != null)
            {
                createdCell.CopyOtherGridCell(cell);
            }

            newObject.transform.position = cell.transform.position;

            referenceObject.gameGrid.OverrideSpecificCell(cell.ReturnPosInGridArray(), createdCell, newObject);


            Destroy(cell.gameObject);
        }
    }

    public void SetCurrentTypeOfCellSelected(TypeOfCell typeOfCell)
    {
        currentTypeOfCellSelected = typeOfCell;
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
            currentSpawnerSelected.DisplaySpecificPath(show, index);
        }
    }
    public void CallDeleteSpecificPath(int index)
    {
        if (currentSpawnerSelected)
        {
            currentSpawnerSelected.DeleteSpecificPath(index);
        }
    }

}
