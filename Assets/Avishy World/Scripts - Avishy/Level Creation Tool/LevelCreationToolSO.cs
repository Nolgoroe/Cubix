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
[Serializable]
public struct TypeOfCellColorToColor
{
    public CellTypeColor cellTypeColor;
    public Color color; 
}

[CreateAssetMenu(fileName = "Level Creation Tool", menuName = "ScriptableObjects/LevelCreationToolColors")]
public class LevelCreationToolSO : ScriptableObject
{
    [SerializeField] private List<typeToColorCombo> typeToColorList = new List<typeToColorCombo>();
    [SerializeField] private List<colorToPrefabCombo> colorToPrefabList = new List<colorToPrefabCombo>();
    [SerializeField] private List<typeToMaterialCombo> typeToMaterialList = new List<typeToMaterialCombo>();
    [SerializeField] private List<PathSidesToMesh> pathSidesToMeshes = new List<PathSidesToMesh>();
    
    [SerializeField] private List<TypeOfCellColorToColor> typeOfCellColors = new List<TypeOfCellColorToColor>();

    [ContextMenu("Create Number Of Combo's")]
    private void CreateTypeToColorCombos()
    {
        #region Type To Color
        List<typeToColorCombo> localTypeToColor = new List<typeToColorCombo>();

        if (typeToColorList.Count > 0)
        {
            localTypeToColor.AddRange(typeToColorList);
        }

        typeToColorList.Clear();

        foreach (TypeOfCell cellType in Enum.GetValues(typeof(TypeOfCell)))
        {
            typeToColorCombo typeToColor = new typeToColorCombo();
            typeToColor.typeOfCell = cellType;
            typeToColor.color = new Color(0,0,0,1);

            if (localTypeToColor.Count > 0)
            {
                foreach (typeToColorCombo combo in localTypeToColor)
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
        List<colorToPrefabCombo> localColorToPrefab = new List<colorToPrefabCombo>();

        if (colorToPrefabList.Count > 0)
        {
            localColorToPrefab.AddRange(colorToPrefabList);
        }

        colorToPrefabList.Clear();
        foreach (typeToColorCombo typeToColorCombo in typeToColorList)
        {
            colorToPrefabCombo colorToPrefab = new colorToPrefabCombo();
            colorToPrefab.color = typeToColorCombo.color;

            if(localColorToPrefab.Count > 0)
            {
                foreach (colorToPrefabCombo combo in localColorToPrefab)
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
        List<typeToMaterialCombo> localTypeToMaterial = new List<typeToMaterialCombo>();
        
        if(typeToMaterialList.Count > 0)
        {
            localTypeToMaterial.AddRange(typeToMaterialList);
        }

        typeToMaterialList.Clear();

        foreach (TypeOfCell cellType in Enum.GetValues(typeof(TypeOfCell)))
        {
            typeToMaterialCombo typeToMat = new typeToMaterialCombo();
            typeToMat.typeOfCell = cellType;

            if (localTypeToMaterial.Count > 0)
            {
                foreach (typeToMaterialCombo combo in localTypeToMaterial)
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
        List<PathSidesToMesh> localSidesToMeshList = new List<PathSidesToMesh>();

        if (pathSidesToMeshes.Count > 0)
        {
            localSidesToMeshList.AddRange(pathSidesToMeshes);
        }

        pathSidesToMeshes.Clear();

        foreach (PathSides side in Enum.GetValues(typeof(PathSides)))
        {
            PathSidesToMesh sidesToMesh = new PathSidesToMesh();
            sidesToMesh.side = side;

            if (localSidesToMeshList.Count > 0)
            {
                foreach (PathSidesToMesh combo in localSidesToMeshList)
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

    [ContextMenu("Create Cell type Combo's")]
    private void CreateCellTypeCombos()
    {
        #region Cell Types Color To Color
        List<TypeOfCellColorToColor> localCellTypeColorList = new List<TypeOfCellColorToColor>();

        if (typeOfCellColors.Count > 0)
        {
            localCellTypeColorList.AddRange(typeOfCellColors);
        }

        typeOfCellColors.Clear();

        foreach (CellTypeColor cellTypeColor in Enum.GetValues(typeof(CellTypeColor)))
        {
            TypeOfCellColorToColor newCombo = new TypeOfCellColorToColor();
            newCombo.cellTypeColor = cellTypeColor;

            if (localCellTypeColorList.Count > 0)
            {
                foreach (TypeOfCellColorToColor localColorCombo in localCellTypeColorList)
                {
                    if (newCombo.cellTypeColor == localColorCombo.cellTypeColor)
                    {
                        newCombo.color = localColorCombo.color;
                        newCombo.color.a = 1;
                    }
                }
            }

            typeOfCellColors.Add(newCombo);
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
    public Color ReturnColorByCellTypeColor(CellTypeColor cellTypeColor)
    {
        TypeOfCellColorToColor foundCombo = typeOfCellColors.Where(combo => combo.cellTypeColor == cellTypeColor).FirstOrDefault();

        return foundCombo.color;
    }
}
