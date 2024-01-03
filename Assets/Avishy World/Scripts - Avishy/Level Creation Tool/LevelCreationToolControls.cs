using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfCell
{
    enemyPath,
    enemySpawner,
    Obstacle,
    PlayerBase,
    None
}

public class LevelCreationToolControls : MonoBehaviour
{
    [Header("Needed References")]
    [SerializeField] private LevelCreationToolColorSO levelCreationToolColors;

    [Header("Automatic References")]
    [SerializeField] private GameGrid gameGrid;

    [Header("Cell Detection")]
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private GridCell currentCellSelected;

    [Header("Draw Data")]
    [SerializeField] private TypeOfCell currentTypeOfCellSelected;


    private Vector3 positionOfMouse;

    void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
    }

    private void Update()
    {
        MouseOverGridCell();

        if (currentCellSelected)
        {
            Vector3 cellPos = currentCellSelected.transform.position;
            cellIndicator.transform.position = new Vector3(cellPos.x, cellPos.y + 0.75f, cellPos.z);

            if (Input.GetMouseButton(0))
            {
                currentCellSelected.ChangeCellColor(levelCreationToolColors.ConvertTypeToColor(currentTypeOfCellSelected));
            }
            if (Input.GetMouseButton(1))
            {
                currentCellSelected.ChangeCellColor(levelCreationToolColors.ConvertTypeToColor(TypeOfCell.None));
            }
        }
    }

    private GridCell MouseOverGridCell()
    {
        currentCellSelected = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        positionOfMouse = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, gridCellLayer))
        {
            hitInfo.transform.TryGetComponent<GridCell>(out currentCellSelected);
        }

        return currentCellSelected;
    }


    [ContextMenu("Generate The Level")]
    public void GenerateTheLevel()
    {
        foreach (GridCell cell in gameGrid.ReturnCellsArray())
        {
            GameObject toSpawn = levelCreationToolColors.SpawnPrefabByColor(cell.returnCellColor());

            if (toSpawn == null) continue;

            GameObject newObject = Instantiate(toSpawn, cell.transform.parent);

            newObject.TryGetComponent<GridCell>(out GridCell createdCell);
            if(createdCell != null)
            {
                createdCell.CopyOtherGridCell(cell);
            }

            newObject.transform.position = cell.transform.position;

            gameGrid.OverrideSpecificCell(cell.ReturnPosInGridArray(), createdCell, newObject);


            Destroy(cell.gameObject);
        }
    }


    public void SetCurrentTypeOfCellSelected(TypeOfCell typeOfCell)
    {
        currentTypeOfCellSelected = typeOfCell;
    }
}
