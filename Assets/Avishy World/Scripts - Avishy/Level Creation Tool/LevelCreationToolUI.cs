using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif 

public class LevelCreationToolUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ToolReferencerObject referenceObject;
    [SerializeField] string prefabPath;

    [Header("Drawing")]
    [SerializeField] TMP_Dropdown dropDownCellType;

    [Header("Grid Generation")]
    [SerializeField] TMP_InputField inputHeight;
    [SerializeField] TMP_InputField inputWidth;
    [SerializeField] TMP_InputField inputSpacing;

    [Header("Waypoints")]
    [SerializeField] TMP_InputField inputWaypointIndex;

    [Header("Level Data")]
    [SerializeField] TMP_Dropdown dropDownLevelList;

    [Header("General")]
    [SerializeField] TMP_InputField levelName;
    [SerializeField] string decidedLevelName;

    private void Start()
    {
        PopupateCellTypesDropdownList();
        PopupateLevelPrefabList();
    }

    public void Dropdown_IndexChangedCellType()
    {
        int index = dropDownCellType.value;
        TypeOfCell typeOfCell = (TypeOfCell)index;
        referenceObject.controls.SetCurrentTypeOfCellSelected(typeOfCell);
    }
    public void Dropdown_IndexChangedLevel()
    {
        int index = dropDownCellType.value;
        TypeOfCell typeOfCell = (TypeOfCell)index;
        referenceObject.controls.SetCurrentTypeOfCellSelected(typeOfCell);
    }
    public void SetDropdownToValue(int value)
    {
        dropDownCellType.value = value;
    }
    public void UpdateGridSpawnData()
    {
        int height = 0;
        int width = 0;
        int spacing = 0;

        int.TryParse(inputHeight.text, out height);
        int.TryParse(inputWidth.text, out width);
        int.TryParse(inputSpacing.text, out spacing);

        referenceObject.toolGameGrid.SetGridParamsFromUI(height, width, spacing);
    }
    public void UpdateLevelName()
    {
        decidedLevelName = levelName.text;
    }

    public void DisplayEnemySpawnerCellData(ToolEnemySpawnerCell cell)
    {
        //additionalDataParent.SetActive(true);
    }
    public void SaveWaypointsPathButton()
    {
        referenceObject.controls.CallSavePathCreated();
    }


    public void ShowPathByIndex(bool show)
    {
        int index = -1;

        int.TryParse(inputWaypointIndex.text, out index);

        if (index <= -1) return;

        referenceObject.controls.CallDisplaySpecificPath(show, index);
    }
    public void DeletePathByIndex()
    {
        int index = -1;

        int.TryParse(inputWaypointIndex.text, out index);

        if (index <= -1) return;

        referenceObject.controls.CallDeleteSpecificPath(index);
    }



    public void LoadSelectedLevel()
    {
        int index = dropDownLevelList.value;
        
        if(referenceObject.toolGameGrid)
        {
            Destroy(referenceObject.toolGameGrid.gameObject);
        }

        GameObject go = Instantiate(referenceObject.levelList[index]);
        levelName.text = referenceObject.levelList[index].name;

        ToolGameGrid grid;
        go.TryGetComponent<ToolGameGrid>(out grid);

        if(grid)
        {
            Camera.main.transform.position = new Vector3(0, grid.transform.position.y + 20, grid.transform.position.z - 20);
        }
    }

    public void ResetLevelName()
    {
        levelName.text = "";
    }

#if UNITY_EDITOR
    public void CreatePrefabFromGridTool()
    {
        string path = prefabPath + "/"+ decidedLevelName +".prefab";
        Debug.Log(path);

        if(decidedLevelName == "")
        {
            Debug.LogError("No Level Name");
            return;
        }
        PrefabUtility.SaveAsPrefabAsset(referenceObject.toolGameGrid.gameObject, path);

        PopupateLevelPrefabList();
    }
#endif

    private void PopupateCellTypesDropdownList()
    {
        string[] enumName = Enum.GetNames(typeof (TypeOfCell));
        List<string> nameList = new List<string>(enumName);

        dropDownCellType.AddOptions(nameList);
    }
    private void PopupateLevelPrefabList()
    {
        referenceObject.LoadLevelsToList();

        List<string> namesList = referenceObject.levelList.Select(gameObject => gameObject.name).ToList();

        dropDownLevelList.ClearOptions();
        dropDownLevelList.AddOptions(namesList);
    }
}
