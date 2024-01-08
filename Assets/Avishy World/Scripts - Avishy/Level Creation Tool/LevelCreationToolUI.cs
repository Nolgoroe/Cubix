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
    public const string pathToLevelToolPrefabFolder = "Assets/Avishy World/Tool Level Prefabs";
    public const string pathToGameLevelPrefabFolder = "Assets/Avishy World/Game Level Prefabs";

    [Header("Drawing")]
    [SerializeField] TMP_Dropdown dropDownCellType;

    [Header("Grid Generation")]
    [SerializeField] TMP_InputField inputHeight;
    [SerializeField] TMP_InputField inputWidth;
    [SerializeField] TMP_InputField inputSpacing;

    [Header("Waypoints")]
    [SerializeField] TMP_InputField inputWaypointIndex;
    [SerializeField] Button deleteWaypointsButton;
    [SerializeField] TMP_Text amountOfWaypoints;

    [Header("Level Data")]
    [SerializeField] TMP_Dropdown dropDownLevelList;

    [Header("General")]
    [SerializeField] TMP_InputField levelName;
    [SerializeField] string decidedLevelName;
    [SerializeField] TMP_Text systemText;

    [Header("Build Mode")]
    [SerializeField] Toggle inBuildModeToggle;

    private void Start()
    {
        PopupateCellTypesDropdownList();
        PopupateLevelPrefabList();

        StartCoroutine(DisplaySystemMessage("Welcome to our level creation tool! have fun!"));
    }

    public void Dropdown_IndexChangedCellType()
    {
        int index = dropDownCellType.value;
        TypeOfCell typeOfCell = (TypeOfCell)index;
        ToolReferencerObject.Instance.controls.SetCurrentTypeOfCellSelected(typeOfCell);
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

        ToolReferencerObject.Instance.toolGameGrid.SetGridParamsFromUI(height, width, spacing);
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
        ToolReferencerObject.Instance.controls.CallSavePathCreated();
    }


    public void ShowPathByIndex(bool show)
    {
        int index = -1;

        int.TryParse(inputWaypointIndex.text, out index);

        if (index <= -1) return;

        ToolReferencerObject.Instance.controls.CallDisplaySpecificPath(show, index);
    }
    public void DeletePathByIndex()
    {
        int index = -1;

        int.TryParse(inputWaypointIndex.text, out index);

        if (index <= -1) return;

        ToolReferencerObject.Instance.controls.CallDeleteSpecificPath(index);

        deleteWaypointsButton.interactable = false;

        StartCoroutine(ReActivateButton(2, deleteWaypointsButton));
    }

    private IEnumerator ReActivateButton(int time, Button button)
    {
        yield return new WaitForSeconds(time);
        button.interactable = true;
    }



    public void LoadSelectedLevel()
    {
        int index = dropDownLevelList.value;
        
        if(ToolReferencerObject.Instance.toolGameGrid)
        {
            Destroy(ToolReferencerObject.Instance.toolGameGrid.gameObject);
        }

        GameObject go = Instantiate(ToolReferencerObject.Instance.levelList[index]);
        levelName.text = ToolReferencerObject.Instance.levelList[index].name;

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

    public void CallDisplaySystemMessage(string message)
    {
        StartCoroutine(DisplaySystemMessage(message));
    }
    private IEnumerator DisplaySystemMessage(string message)
    {
        systemText.text = message;
        systemText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        systemText.gameObject.SetActive(false);

    }
    public void DisplayAmountOfPathsOnSpawner(int amount)
    {
        amountOfWaypoints.text = "Amount of waypoint lists: " + amount.ToString();
    }

    public void CallCreatePrefabFromGridTool()
    {
        StartCoroutine(CreatePrefabFromGridTool());
    }

#if UNITY_EDITOR
    public IEnumerator CreatePrefabFromGridTool()
    {
        if (decidedLevelName == "")
        {
            Debug.LogError("No Level Name");
            StartCoroutine(DisplaySystemMessage("Must set a level name before saving!"));
            yield break;
        }

        ToolReferencerObject.Instance.toolGameGrid.CleanupBeforePrefab();
        yield return new WaitForSeconds(2); //VERY TEMP HARDCODED

        string levelName = "/" + decidedLevelName + ".prefab";
        string[] paths = new string[] { pathToLevelToolPrefabFolder + levelName,
                                        pathToGameLevelPrefabFolder + levelName };


        foreach (string path in paths)
        {
            PrefabUtility.SaveAsPrefabAsset(ToolReferencerObject.Instance.toolGameGrid.gameObject, path);
        }

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
        ToolReferencerObject.Instance.LoadLevelsToList();

        List<string> namesList = ToolReferencerObject.Instance.levelList.Select(gameObject => gameObject.name).ToList();

        dropDownLevelList.ClearOptions();
        dropDownLevelList.AddOptions(namesList);
    }

    public void ToggleBuildModeToggle(bool isOn)
    {
        inBuildModeToggle.isOn = isOn;
    }
}
