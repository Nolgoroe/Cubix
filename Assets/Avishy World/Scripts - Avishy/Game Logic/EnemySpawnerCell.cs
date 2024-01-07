using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerCell : GridCell
{
    [SerializeField] private List<EnemyPath> enemyPaths;




























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
}
