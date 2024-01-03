using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class LevelCreationToolUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Dropdown dropDown;
    [SerializeField] TMP_InputField inputHeight;
    [SerializeField] TMP_InputField inputWidth;
    [SerializeField] TMP_InputField inputSpacing;
    [SerializeField] LevelCreationToolControls controls;
    [SerializeField] GameGrid gameGrid;

    private void Start()
    {
        PopupateDropdownList();
    }

    public void Dropdown_IndexChanged()
    {
        int index = dropDown.value;
        TypeOfCell typeOfCell = (TypeOfCell)index;
        controls.SetCurrentTypeOfCellSelected(typeOfCell);
    }
    public void UpdateGridSpawnData()
    {
        int height = int.Parse(inputHeight.text);
        int width = int.Parse(inputWidth.text);
        int spacing = int.Parse(inputSpacing.text);

        gameGrid.SetGridParamsFromUI(height, width, spacing);
    }

    private void PopupateDropdownList()
    {
        string[] enumName = Enum.GetNames(typeof (TypeOfCell));
        List<string> nameList = new List<string>(enumName);

        dropDown.AddOptions(nameList);
    }
}
