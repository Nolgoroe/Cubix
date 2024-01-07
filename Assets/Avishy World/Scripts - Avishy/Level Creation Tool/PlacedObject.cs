using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    [SerializeField] private BuildingsSO buildingSO;
    [SerializeField] Vector2Int originPoint;
    private Dir dir;

    public static PlacedObject Create(Vector3 worldPos, Vector2Int origin, Dir dir, BuildingsSO buildingSO, Transform parent)
    {
        //This gives us the rotation of the object by it's direction. we only rotate around the Y axis.
        // example - if we want an object to look life - it's rotation should be 90 on the Y.
        Quaternion RotationByDir = Quaternion.Euler(0, buildingSO.GetRotationAngle(dir), 0);

        GameObject createdBuilding = Instantiate(buildingSO.buildingPrefab, worldPos, RotationByDir, parent);

        PlacedObject placedObject = createdBuilding.GetComponent<PlacedObject>();
        placedObject.buildingSO = buildingSO;
        placedObject.originPoint = origin;
        placedObject.dir = dir;

        return placedObject;
    }
    public static PlacedObject Create(Vector3 worldPos, Dir dir, BuildingsSO buildingSO, Transform parent)
    {
        //This gives us the rotation of the object by it's direction. we only rotate around the Y axis.
        // example - if we want an object to look life - it's rotation should be 90 on the Y.
        Quaternion RotationByDir = Quaternion.Euler(0, buildingSO.GetRotationAngle(dir), 0);

        GameObject createdBuilding = Instantiate(buildingSO.buildingPrefab, worldPos, RotationByDir, parent);

        PlacedObject placedObject = createdBuilding.GetComponent<PlacedObject>();
        placedObject.buildingSO = buildingSO;
        placedObject.dir = dir;

        return placedObject;
    }

    public List<Vector2Int> GetGridPositionsList()
    {
        return buildingSO.GetGridPositionList(originPoint, dir);
    }
    public void DestroySelf()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnMouseOver()
    {
        if (buildingSO.snapToGrid) return; //here we also need to take care of grid elements, not just spawned meshes.

        if(Input.GetMouseButtonDown(1))
        {
            DestroySelf();
        }
    }
}
