using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    [SerializeField] private GameObject visual;
    [SerializeField] private BuildingsSO currentBuilding;
    [SerializeField] private bool isActive;

    private void LateUpdate() {
        if (!isActive) return;

        ToolGridCell currentCell = ToolReferencerObject.Instance.controls.ReturnCurrentCellHovered();

        if(currentCell == null) return;

        BuildingsSO.Dir currentDir = ToolReferencerObject.Instance.controls.ReturnCurrentDir();

        if (currentBuilding.snapToGrid)
        {

            Vector2 rotationOffset = currentBuilding.GetRotationOffset(currentDir); // gets the amount on X and Y that the object needs to move according to it's dircetion and size
            Vector3 poisitonToAddByRotationOffset = new Vector3(rotationOffset.x, 0, rotationOffset.y); //we only move objects on X and Z.
            Vector3 correctionToCellPivot = new Vector3(-0.5f, 0, -0.5f); // we want to force the pivot of the building to be on the EDGE of a cell. this fixes rotations into slots.

            Vector3 buildingPos = correctionToCellPivot +
                    currentCell.transform.position +
                    currentBuilding.buildingPrefab.transform.position +
                    poisitonToAddByRotationOffset;

            transform.position = Vector3.Lerp(transform.position, buildingPos, Time.deltaTime * 15f);


            Quaternion RotationByDir = Quaternion.Euler(0, currentBuilding.GetRotationAngle(currentDir), 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, RotationByDir, Time.deltaTime * 15f);
        }
        else
        {
            Vector3 worldMousePos = ToolReferencerObject.Instance.controls.MouseOverWorldNormal();
            Vector3 buildingPos = worldMousePos + currentBuilding.buildingPrefab.transform.position;
            transform.position = Vector3.Lerp(transform.position, buildingPos, Time.deltaTime * 15f);

            Quaternion RotationByDir = Quaternion.Euler(0, currentBuilding.GetRotationAngle(currentDir), 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, RotationByDir, Time.deltaTime * 15f);
        }
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        if (currentBuilding != null) {
            visual = Instantiate(currentBuilding.visualPrefab, Vector3.zero, Quaternion.identity);
            visual.transform.parent = transform;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localEulerAngles = Vector3.zero;
        }
    }

    public void ToggleGhost()
    {
        isActive = !isActive;

        RefreshVisual();

        if(!isActive)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
    }

    public void SetCurrentBuilding(BuildingsSO toPlace)
    {
        currentBuilding = toPlace;
        RefreshVisual();
    }

}

