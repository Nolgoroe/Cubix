using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public struct typeToColorCombo
{
    public TypeOfCell typeOfCell;
    public Color color;
}

[Serializable]
public struct colorToPrefabCombo
{
    public Color color;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "Level Creation Tool", menuName = "ScriptableObjects/LevelCreationToolColors")]
public class LevelCreationToolColorSO : ScriptableObject
{
    [SerializeField] private List<typeToColorCombo> typeToColorList = new List<typeToColorCombo>();
    [SerializeField] private List<colorToPrefabCombo> colorToPrefabList = new List<colorToPrefabCombo>();

    [ContextMenu("Create Number Of Combo's")]
    private void CreateTypeToColorCombos()
    {
        List<typeToColorCombo> tempList = new List<typeToColorCombo>();

        if (typeToColorList.Count > 0)
        {
            foreach (typeToColorCombo combo in typeToColorList)
            {
                tempList.Add(combo);
            }
        }

        typeToColorList.Clear();

        foreach (TypeOfCell cellType in Enum.GetValues(typeof(TypeOfCell)))
        {
            typeToColorCombo typeToColor = new typeToColorCombo();
            typeToColor.typeOfCell = cellType;
            typeToColor.color = new Color(0,0,0,1);

            if (tempList.Count > 0)
            {
                foreach (typeToColorCombo combo in tempList)
                {
                    if (cellType == combo.typeOfCell)
                    {
                        typeToColor.color = combo.color;
                    }
                }
            }

            typeToColorList.Add(typeToColor);
        }


        colorToPrefabList.Clear();
        foreach (typeToColorCombo combo in typeToColorList)
        {
            colorToPrefabCombo colorToPrefab = new colorToPrefabCombo();
            colorToPrefab.color = combo.color;

            colorToPrefabList.Add(colorToPrefab);
        }

    }

    public Color ConvertTypeToColor(TypeOfCell typeToSearch)
    {
        typeToColorCombo foundCombo = typeToColorList.Where(combo => combo.typeOfCell == typeToSearch).FirstOrDefault();
        return foundCombo.color;
    }

    public GameObject SpawnPrefabByColor(Color color)
    {
        colorToPrefabCombo foundCombo = colorToPrefabList.Where(combo => combo.color == color).FirstOrDefault();
        return foundCombo.prefab;
    }

}
