using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyPath
{
    public List<Vector2Int> waypoints;
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
    public void AddToEnemyPath(ToolGridCell toAdd, Vector2Int ToAddPos)
    {
        if (currentPathBeingCreated.waypoints.Contains(ToAddPos) || toAdd.ReturnTypeOfCell() == TypeOfCell.enemySpawner) return;

        if(currentPathBeingCreated.waypoints.Count == 0)
        {
            DisplayAsWaypoint(true);
            currentPathBeingCreated.waypoints.Add(positionXYInGridArray);

            if(ReturnSpawnedWaypoint())
                waypointTransforms.Add(ReturnSpawnedWaypoint());
        }
        toAdd.DisplayAsWaypoint(true);
        currentPathBeingCreated.waypoints.Add(ToAddPos);

        if (ReturnSpawnedWaypoint())
            waypointTransforms.Add(toAdd.ReturnSpawnedWaypoint());

        RotateWaypoints(); //TEMP HERE!
    }
    public IEnumerator RemoveFromEnemyPath(ToolGridCell toRemove)
    {
        if (currentPathBeingCreated.waypoints.Count <= 0) yield break;

        if (toRemove.ReturnSpawnedWaypoint())
            waypointTransforms.Remove(toRemove.ReturnSpawnedWaypoint());

        currentPathBeingCreated.waypoints.Remove(toRemove.ReturnPosInGridArray());

        yield return new WaitForSeconds(0.5f);

        toRemove.DisplayAsWaypoint(false);

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
            newPath.waypoints = new List<Vector2Int>();
            newPath.waypoints.AddRange(currentPathBeingCreated.waypoints);
            enemyPaths.Add(newPath);

            foreach (Vector2Int waypoint in currentPathBeingCreated.waypoints)
            {
                ToolGridCell cell = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray()[waypoint.x, waypoint.y];
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

        waypointTransforms.Clear();
        yield return new WaitForSeconds(0.5f);

        if (show)
        {
            if (currentPathBeingCreated.waypoints.Count > 0)
            {
                foreach (Vector2Int waypoint in currentPathBeingCreated.waypoints)
                {
                    ToolGridCell cell = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray()[waypoint.x, waypoint.y];

                    cell.DisplayAsWaypoint(false);
                }
                yield return new WaitForSeconds(0.5f);
            }

            currentPathBeingCreated = new EnemyPath();
            currentPathBeingCreated.waypoints = new List<Vector2Int>();

            yield return new WaitForSeconds(0.5f);
            currentPathBeingCreated.waypoints.AddRange(enemyPaths[index].waypoints);
            
            currentPathIndex = index;
        }
        else
        {
            foreach (Vector2Int cell in enemyPaths[index].waypoints)
            {
                currentPathBeingCreated.waypoints.Remove(cell);
            }

            yield return new WaitForSeconds(0.5f);

            currentPathBeingCreated = new EnemyPath();
            currentPathBeingCreated.waypoints = new List<Vector2Int>();
        }

        foreach (Vector2Int waypoint in enemyPaths[index].waypoints)
        {
            ToolGridCell cell = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray()[waypoint.x, waypoint.y];

            cell.DisplayAsWaypoint(show);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (Vector2Int waypoint in currentPathBeingCreated.waypoints)
        {
            ToolGridCell cell = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray()[waypoint.x, waypoint.y];

            waypointTransforms.Add(cell.ReturnSpawnedWaypoint());
        }

        if(show)
        {
            RotateWaypoints();
        }

    }

    public IEnumerator ClearAllTempData()
    {
        if (currentPathBeingCreated.waypoints.Count > 0)
        {
            foreach (Vector2Int waypoint in currentPathBeingCreated.waypoints)
            {
                ToolGridCell cell = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray()[waypoint.x, waypoint.y];

                cell.DisplayAsWaypoint(false);
            }

            yield return new WaitForSeconds(0.5f);
        }

        waypointTransforms.Clear();
        currentPathBeingCreated = new EnemyPath();
        currentPathBeingCreated.waypoints = new List<Vector2Int>();
    }

    public IEnumerator DeleteSpecificPath(int index)
    {
        if (enemyPaths.Count <= 0 || index > enemyPaths.Count - 1) yield break;

        yield return StartCoroutine(DisplaySpecificPath(false, index));

        enemyPaths.Remove(enemyPaths[index]);
    }

    public List<EnemyPath> ReturnEnemyPaths()
    {
        return enemyPaths;
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
