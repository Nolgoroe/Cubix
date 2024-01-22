using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        dangerIcon = transform.GetChild(0); // temp
    }
    protected override void Start()
    {
    }

    private void SpawnEnemy(GameObject enemyPrefab, int followPathIndex)
    {
        if (enemyPaths.Count == 0) return;

        EnemyParent enemy = new EnemyParent();
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

    public void DisplayDangerIcon(bool display)
    {
        dangerIcon.gameObject.SetActive(display);
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
