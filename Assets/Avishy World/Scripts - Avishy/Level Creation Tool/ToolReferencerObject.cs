using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ToolReferencerObject : MonoBehaviour
{
    public LevelCreationToolControls controls;
    public LevelCreationToolSO levelCreationToolSO;
    public LevelCreationToolUI toolUI;

    [Header ("Automatic")]
    public ToolGameGrid gameGrid;
    public List<GameObject> levelList;

    private void Update()
    {
        if (gameGrid == null)
        {
            gameGrid = FindObjectOfType<ToolGameGrid>();
        }
    }

    [ContextMenu("Load Levels To List")]
    public void LoadLevelsToList()
    {
#if UNITY_EDITOR

        levelList.Clear();

        string[] foldersToSearchIn = new string[] { "Assets/Avishy World/Level Prefabs" };

        levelList = AssetDatabase.FindAssets("t:prefab", foldersToSearchIn)
            .Select(p => AssetDatabase.GUIDToAssetPath(p))
            .Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(g))
            .ToList();

#endif
    }
}
