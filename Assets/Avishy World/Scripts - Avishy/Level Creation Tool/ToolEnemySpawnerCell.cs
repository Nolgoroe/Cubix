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

    [SerializeField] private int currentPathIndex = -1;

    [SerializeField] private EnemyPath currentPathBeingCreated;
    [SerializeField] private List<Transform> waypointTransforms;

    private void Start()
    {
        waypointTransforms = new List<Transform>();
    }
    public void AddToEnemyPath(ToolGridCell toAdd)
    {
        if (currentPathBeingCreated.waypoints.Contains(toAdd) || toAdd.ReturnTypeOfCell() == TypeOfCell.enemySpawner) return;

        if(currentPathBeingCreated.waypoints.Count == 0)
        {
            DisplayAsWaypoint(true);
            currentPathBeingCreated.waypoints.Add(this);

            if(ReturnSpawnedWaypoint())
                waypointTransforms.Add(ReturnSpawnedWaypoint());
        }
        toAdd.DisplayAsWaypoint(true);
        currentPathBeingCreated.waypoints.Add(toAdd);

        if (ReturnSpawnedWaypoint())
            waypointTransforms.Add(toAdd.ReturnSpawnedWaypoint());

        RotateWaypoints(); //TEMP HERE!
    }
    public void RemoveFromEnemyPath(ToolGridCell toRemove)
    {
        if (currentPathBeingCreated.waypoints.Count <= 0) return;

        if (toRemove.ReturnSpawnedWaypoint())
            waypointTransforms.Remove(toRemove.ReturnSpawnedWaypoint());

        toRemove.DisplayAsWaypoint(false);
        currentPathBeingCreated.waypoints.Remove(toRemove);

        RotateWaypoints(); //TEMP HERE!
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
        waypointTransforms.Clear();
        currentPathIndex = -1;
    }

    public IEnumerator DisplaySpecificPath(bool show, int index)
    {
        if (enemyPaths.Count <= 0 || index > enemyPaths.Count - 1) yield break;

        foreach (EnemyPath path in enemyPaths)
        {
            foreach (ToolGridCell cell in path.waypoints)
            {
                cell.DisplayAsWaypoint(false);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (currentPathBeingCreated.waypoints.Count > 0)
        {
            foreach (ToolGridCell cell in currentPathBeingCreated.waypoints)
            {
                cell.DisplayAsWaypoint(false);
            }
            yield return new WaitForSeconds(0.5f);
        }


        if (show)
        {
            currentPathBeingCreated = new EnemyPath();
            currentPathBeingCreated.waypoints = new List<ToolGridCell>();

            currentPathBeingCreated.waypoints.AddRange(enemyPaths[index].waypoints);
            
            currentPathIndex = index;
        }
        else
        {
            foreach (ToolGridCell cell in enemyPaths[index].waypoints)
            {
                currentPathBeingCreated.waypoints.Remove(cell);
            }

            currentPathBeingCreated = new EnemyPath();
            currentPathBeingCreated.waypoints = new List<ToolGridCell>();
        }

        foreach (ToolGridCell cell in enemyPaths[index].waypoints)
        {
            cell.DisplayAsWaypoint(show);
        }

        yield return new WaitForSeconds(0.5f);

        waypointTransforms.Clear();

        foreach (ToolGridCell cell in currentPathBeingCreated.waypoints)
        {
            waypointTransforms.Add(cell.ReturnSpawnedWaypoint());
        }

    }

    public IEnumerator ClearAllTempData()
    {
        if (currentPathBeingCreated.waypoints.Count > 0)
        {
            foreach (ToolGridCell cell in currentPathBeingCreated.waypoints)
            {
                cell.DisplayAsWaypoint(false);
            }

            yield return new WaitForSeconds(0.5f);
        }

        waypointTransforms.Clear();
        currentPathBeingCreated = new EnemyPath();
        currentPathBeingCreated.waypoints = new List<ToolGridCell>();
    }

    public void DeleteSpecificPath(int index)
    {
        if (enemyPaths.Count <= 0 || index > enemyPaths.Count - 1) return;

        StartCoroutine(DisplaySpecificPath(false, index));

        enemyPaths.Remove(enemyPaths[index]);
    }

    [ContextMenu("Rotate now!")]
    private void RotateWaypoints()
    {
        for (int i = 0; i < waypointTransforms.Count; i++)
        {
            if (i + 1 >= waypointTransforms.Count) return;
            waypointTransforms[i].transform.LookAt(waypointTransforms[i + 1]);
        }
    }
}
