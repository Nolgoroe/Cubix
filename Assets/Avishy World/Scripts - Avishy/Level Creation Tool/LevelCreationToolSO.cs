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

[Serializable]
public struct typeToMaterialCombo
{
    public TypeOfCell typeOfCell;
    public Material mat;
}

[Serializable]
public struct PathSidesToMesh
{
    public PathSides side;
    public Mesh MeshPathSide; 
}

[CreateAssetMenu(fileName = "Level Creation Tool", menuName = "ScriptableObjects/LevelCreationToolColors")]
public class LevelCreationToolSO : ScriptableObject
{
    [SerializeField] private List<typeToColorCombo> typeToColorList = new List<typeToColorCombo>();
    [SerializeField] private List<colorToPrefabCombo> colorToPrefabList = new List<colorToPrefabCombo>();
    [SerializeField] private List<typeToMaterialCombo> typeToMaterialList = new List<typeToMaterialCombo>();
    [SerializeField] private List<PathSidesToMesh> pathSidesToMeshes = new List<PathSidesToMesh>();

    [ContextMenu("Create Number Of Combo's")]
    private void CreateTypeToColorCombos()
    {
        #region Type To Color
        List<typeToColorCombo> tempTypeToColor = new List<typeToColorCombo>();

        if (typeToColorList.Count > 0)
        {
            tempTypeToColor.AddRange(typeToColorList);
        }

        typeToColorList.Clear();

        foreach (TypeOfCell cellType in Enum.GetValues(typeof(TypeOfCell)))
        {
            typeToColorCombo typeToColor = new typeToColorCombo();
            typeToColor.typeOfCell = cellType;
            typeToColor.color = new Color(0,0,0,1);

            if (tempTypeToColor.Count > 0)
            {
                foreach (typeToColorCombo combo in tempTypeToColor)
                {
                    if (cellType == combo.typeOfCell)
                    {
                        typeToColor.color = combo.color;
                    }
                }
            }

            typeToColorList.Add(typeToColor);
        }
        #endregion


        #region Color To Prefab
        List<colorToPrefabCombo> tempColorToPrefab = new List<colorToPrefabCombo>();

        if (colorToPrefabList.Count > 0)
        {
            tempColorToPrefab.AddRange(colorToPrefabList);
        }

        colorToPrefabList.Clear();
        foreach (typeToColorCombo typeToColorCombo in typeToColorList)
        {
            colorToPrefabCombo colorToPrefab = new colorToPrefabCombo();
            colorToPrefab.color = typeToColorCombo.color;

            if(tempColorToPrefab.Count > 0)
            {
                foreach (colorToPrefabCombo combo in tempColorToPrefab)
                {
                    if (combo.color == colorToPrefab.color)
                    {
                        colorToPrefab.prefab = combo.prefab;
                    }
                }
            }


            colorToPrefabList.Add(colorToPrefab);
        }

        #endregion

        #region Type To Material
        List<typeToMaterialCombo> tempTypeToMaterial = new List<typeToMaterialCombo>();
        
        if(typeToMaterialList.Count > 0)
        {
            tempTypeToMaterial.AddRange(typeToMaterialList);
        }

        typeToMaterialList.Clear();

        foreach (TypeOfCell cellType in Enum.GetValues(typeof(TypeOfCell)))
        {
            typeToMaterialCombo typeToMat = new typeToMaterialCombo();
            typeToMat.typeOfCell = cellType;

            if (tempTypeToMaterial.Count > 0)
            {
                foreach (typeToMaterialCombo combo in tempTypeToMaterial)
                {
                    if (cellType == combo.typeOfCell)
                    {
                        typeToMat.mat = combo.mat;
                    }
                }
            }

            typeToMaterialList.Add(typeToMat);
        }

        #endregion

        #region Path Type To Mesh
        List<PathSidesToMesh> tempSidesToMeshList = new List<PathSidesToMesh>();

        if (pathSidesToMeshes.Count > 0)
        {
            tempSidesToMeshList.AddRange(pathSidesToMeshes);
        }

        pathSidesToMeshes.Clear();

        foreach (PathSides side in Enum.GetValues(typeof(PathSides)))
        {
            PathSidesToMesh sidesToMesh = new PathSidesToMesh();
            sidesToMesh.side = side;

            if (tempSidesToMeshList.Count > 0)
            {
                foreach (PathSidesToMesh combo in tempSidesToMeshList)
                {
                    if (combo.side == side)
                    {
                        sidesToMesh.MeshPathSide = combo.MeshPathSide;
                    }
                }
            }


            pathSidesToMeshes.Add(sidesToMesh);
        }
        #endregion

    }

    public Color ConvertTypeToColor(TypeOfCell typeToSearch)
    {
        typeToColorCombo foundCombo = typeToColorList.Where(combo => combo.typeOfCell == typeToSearch).FirstOrDefault();
        return foundCombo.color;
    }

    public GameObject SpawnPrefabByColor(Color color)
    {
        colorToPrefabCombo foundCombo = colorToPrefabList.Where(combo => ColorUtility.ToHtmlStringRGBA(combo.color) == ColorUtility.ToHtmlStringRGBA(color)).FirstOrDefault();
        return foundCombo.prefab;
    }

    public Material ReturnMatByType(TypeOfCell cellType)
    {
        typeToMaterialCombo foundCombo = typeToMaterialList.Where(combo => combo.typeOfCell == cellType).FirstOrDefault();

        return foundCombo.mat;
    }
    public Mesh ReturnPrefabByPathSides(PathSides side)
    {
        PathSidesToMesh foundCombo = pathSidesToMeshes.Where(combo => combo.side == side).FirstOrDefault();

        return foundCombo.MeshPathSide;
    }
}
