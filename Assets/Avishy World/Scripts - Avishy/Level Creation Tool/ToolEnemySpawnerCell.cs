using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct EnemyPath
{
    public List<ToolGridCell> waypoints;
}

public class ToolEnemySpawnerCell : ToolGridCell
{
    [SerializeField] private List<EnemyPath> enemyPaths;
    [SerializeField] private EnemyPath currentPathBeingCreated;

    [SerializeField] private int currentPathIndex = -1;

    public void AddToEnemyPath(ToolGridCell toAdd)
    {
        if (currentPathBeingCreated.waypoints.Contains(toAdd) || toAdd.ReturnTypeOfCell() == TypeOfCell.enemySpawner) return;

        if(currentPathBeingCreated.waypoints.Count == 0)
        {
            currentPathBeingCreated.waypoints.Add(this);
        }

        currentPathBeingCreated.waypoints.Add(toAdd);
    }
    public void RemoveFromEnemyPath(ToolGridCell toRemove)
    {
        if (currentPathBeingCreated.waypoints.Count <= 0) return;

        currentPathBeingCreated.waypoints.Remove(toRemove);
    }

    [ContextMenu("Save Path")]
    public IEnumerator SaveCreatedPath()
    {
        if (currentPathBeingCreated.waypoints == null || currentPathBeingCreated.waypoints.Count <= 1) yield break;

        if (currentPathIndex >= 0 && currentPathIndex < enemyPaths.Count - 1)
        {
            enemyPaths[currentPathIndex].waypoints.Clear();
            enemyPaths[currentPathIndex].waypoints.AddRange(currentPathBeingCreated.waypoints);
        }
        else
        {
            EnemyPath newPath = new EnemyPath();
            newPath.waypoints = new List<ToolGridCell>();
            newPath.waypoints.AddRange(currentPathBeingCreated.waypoints);
            enemyPaths.Add(newPath);

            foreach (ToolGridCell cell in currentPathBeingCreated.waypoints)
            {
                cell.DisplayAsWaypoint(false);
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentPathBeingCreated.waypoints.Clear();
        currentPathIndex = -1;
    }

    public void DisplaySpecificPath(bool show, int index)
    {
        if (enemyPaths.Count <= 0 || index > enemyPaths.Count - 1) return;

        foreach (EnemyPath path in enemyPaths)
        {
            foreach (ToolGridCell cell in path.waypoints)
            {
                cell.DisplayAsWaypoint(false);
            }
        }

        foreach (ToolGridCell cell in enemyPaths[index].waypoints)
        {
            cell.DisplayAsWaypoint(show);
        }

        if(show)
        {
            //currentPathBeingCreated = new EnemyPath();
            //currentPathBeingCreated.waypoints = new List<ToolGridCell>();

            currentPathBeingCreated.waypoints.AddRange(enemyPaths[index].waypoints);
            currentPathIndex = index;
        }
        else
        {
            foreach (ToolGridCell cell in enemyPaths[index].waypoints)
            {
                currentPathBeingCreated.waypoints.Remove(cell);
            }
        }
    }

    public void DeleteSpecificPath(int index)
    {
        if (enemyPaths.Count <= 0 || index > enemyPaths.Count - 1) return;

        DisplaySpecificPath(false, index);

        enemyPaths.Remove(enemyPaths[index]);
    }
}
