using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct EnemyPathCells
{
    public List<GridCell> pathCells;
}

public class EnemySpawnerCell : GridCell
{
    [SerializeField] private List<EnemyPath> enemyPaths;

    [SerializeField] private List<EnemyPathCells> enemyPathcells;
    [SerializeField] Transform dangerIcon;
    [SerializeField] TMP_Text amountText;

    private void Awake()
    {
        dangerIcon = transform.GetChild(0); // temp
        amountText = transform.GetChild(1).GetComponent<TMP_Text>(); // temp
    }
    protected override void Start()
    {
        // nothing on start but still overriding.
    }

    private void SpawnEnemy(GameObject enemyPrefab, int followPathIndex)
    {
        if (enemyPaths.Count == 0) return;

        EnemyParent enemy;
        GameObject go = Instantiate(enemyPrefab, transform.position + enemyPrefab.transform.position, transform.rotation);

        go.TryGetComponent<EnemyParent>(out enemy);


        if(enemy)
        {
            enemy.InitEnemy(enemyPathcells[followPathIndex].pathCells);
        }
    }








    public void InitWaypointData()
    {
        if (enemyPathcells == null || enemyPathcells.Count == 0)
        {
            foreach (EnemyPath V2Path in enemyPaths)
            {
                EnemyPathCells newPath = new EnemyPathCells();
                newPath.pathCells = new List<GridCell>();

                foreach (Vector2Int vector in V2Path.waypoints)
                {
                    newPath.pathCells.Add(GridManager.Instance.ReturnCellFromList(vector));
                }

                enemyPathcells.Add(newPath);
            }
        }
    }

    public void CallSpawnEnemy(GameObject enemyPrefab, int followPathIndex)
    {
        SpawnEnemy(enemyPrefab, followPathIndex);
    }

    public void DisplayTowerIcons(bool display, int amount)
    {
        dangerIcon.gameObject.SetActive(display);

        if(display && amount > 0)
        {
            amountText.gameObject.SetActive(display);
            amountText.text = amount.ToString();
        }

        if(!display)
        {
            amountText.gameObject.SetActive(display);
        }
    }
    public void ChangeTowerEnemyText(int amount)
    {
        amountText.text = amount.ToString();
    }

    public override void CopyDataFromToolCell(ToolGridCell toolGridCell)
    {
        ToolEnemySpawnerCell toolSpawnerCell = toolGridCell as ToolEnemySpawnerCell;
        if(toolSpawnerCell == null)
        {
            Debug.LogError("ERROR here");
            return;
        }

        enemyPaths = new List<EnemyPath>();
        enemyPaths = toolSpawnerCell.ReturnEnemyPaths();

        base.CopyDataFromToolCell(toolGridCell);
    }

    public List<EnemyPathCells> ReturnEnemyPathCellsList()
    {
        return enemyPathcells;
    }
}
