using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ToolReferencerObject : MonoBehaviour
{
    public static ToolReferencerObject Instance;

    public LevelCreationToolControls controls;
    public LevelCreationToolSO levelCreationToolSO;
    public LevelCreationToolUI toolUI;
    public ToolWaveCreator toolWaveCreator;

    [Header ("Automatic")]
    public ToolGameGrid toolGameGrid;
    public GridManager gameGrid;
    public List<GameObject> levelList;
    public List<GameObject> allEnemies;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PopulateAllEnemiesList();
    }

    private void Update()
    {
        if (toolGameGrid == null)
        {
            toolGameGrid = FindObjectOfType<ToolGameGrid>();
        }
        if (gameGrid == null)
        {
            gameGrid = FindObjectOfType<GridManager>();
        }
    }

    [ContextMenu("Load Levels To List")]
    public void LoadLevelsToList()
    {
#if UNITY_EDITOR

        levelList.Clear();

        string[] foldersToSearchIn = new string[] { "Assets/Avishy World/Tool Level Prefabs" };

        levelList = AssetDatabase.FindAssets("t:prefab", foldersToSearchIn)
            .Select(p => AssetDatabase.GUIDToAssetPath(p))
            .Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(g))
            .ToList();

#endif
    }

    private void PopulateAllEnemiesList()
    {
#if UNITY_EDITOR

        allEnemies.Clear();

        string[] foldersToSearchIn = new string[] { "Assets/Avishy World/Prefabs - Avishy/Enemies" };

        allEnemies = AssetDatabase.FindAssets("t:prefab", foldersToSearchIn)
            .Select(p => AssetDatabase.GUIDToAssetPath(p))
            .Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(g))
            .ToList();

#endif
    }

}
