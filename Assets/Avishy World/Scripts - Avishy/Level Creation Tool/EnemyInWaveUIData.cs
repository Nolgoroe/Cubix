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

public class EnemyInWaveUIData : MonoBehaviour
{
    [Header("Wave Creation Tool Data")]
    [SerializeField] TMP_Dropdown enemyTypeDropdown;
    [SerializeField] TMP_InputField amountOfEnemy;
    [SerializeField] TMP_InputField spawnerIndex;
    [SerializeField] TMP_InputField pathIndex;
    [SerializeField] EnemyWaveData localEnemyData;


    private void PopupateEnemyDropdown()
    {
        string[] enumName = Enum.GetNames(typeof(EnemyTypes));
        List<string> nameList = new List<string>(enumName);

        enemyTypeDropdown.AddOptions(nameList);
    }






    public void InitEnemyInWaveUI(EnemyWaveData data)
    {
        PopupateEnemyDropdown();

        localEnemyData = data;

        enemyTypeDropdown.value = (int)data.enemyType;
        amountOfEnemy.text = data.amountOfThisEnemy.ToString();
        spawnerIndex.text = data.enemySpawnerIndex.ToString();
        pathIndex.text = data.enemyPathIndex.ToString();
    }

    public void UpdateEnemyType()
    {
        //called from ui element

        int index = enemyTypeDropdown.value;
        EnemyTypes enemyTypeSelected = (EnemyTypes)index;

        List<GameObject> allEnemies = ToolReferencerObject.Instance.allEnemies;
        localEnemyData.enemyType = EnemyTypes.None;

        foreach (GameObject go in allEnemies)
        {
            EnemyParent enemy;
            go.TryGetComponent<EnemyParent>(out enemy);

            if (enemy)
            {
                if (enemy.RetrunEnemyType() == enemyTypeSelected)
                {
                    localEnemyData.enemyType = enemyTypeSelected;
                    return;
                }
            }
        }
    }
    public void UpdateAmountEnemy()
    {
        //called from ui element

        int amount = 0;

        int.TryParse(amountOfEnemy.text, out amount);

        localEnemyData.amountOfThisEnemy = amount;
    }
    public void UpdateSpawnerToSpawnFrom()
    {
        //called from ui element

        int index = 0;

        int.TryParse(spawnerIndex.text, out index);

        int amountOfSpawners = ToolReferencerObject.Instance.toolGameGrid.ReturnEnemySpawners().Count;
        if (index >= amountOfSpawners)
        {
            ToolReferencerObject.Instance.toolUI.CallDisplaySystemMessage(
                "You must choose an index from the spawners in the levle. " +
                "the number you put was invalid.");
            return;
        }

        localEnemyData.enemySpawnerIndex = index;
    }
    public void UpdatePathToTake()
    {
        //called from ui element
        int index = 0;

        int.TryParse(pathIndex.text, out index);

        List<ToolEnemySpawnerCell> spawners = ToolReferencerObject.Instance.toolGameGrid.ReturnEnemySpawners();
        if (index >= spawners[localEnemyData.enemySpawnerIndex].ReturnEnemyPaths().Count)
        {
            ToolReferencerObject.Instance.toolUI.CallDisplaySystemMessage(
                "You must choose an index from the paths in the spawner. " +
                "the number you put was invalid.");
            return;
        }


        localEnemyData.enemyPathIndex = index;
    }
    public void DeleteThisEnemy()
    {
        //called from ui element
        ToolReferencerObject.Instance.toolWaveCreator.RemoveEnemyFromWave(localEnemyData);

        Destroy(gameObject);
    }
}
