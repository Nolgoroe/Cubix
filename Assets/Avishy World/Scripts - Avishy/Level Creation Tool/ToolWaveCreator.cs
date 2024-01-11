using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif 

public class ToolWaveCreator : MonoBehaviour
{
    [SerializeField] List<WaveData> waveList;
    [SerializeField] int currentWaveSelectedIndex;

    [Header("Tool UI First")]
    [SerializeField] GameObject waveCreatorUI;
    [SerializeField] TMP_InputField waveCountInputText;
    [SerializeField] TMP_InputField currentWaveIndex;

    [Header("Tool UI After first Selection")]
    [SerializeField] Transform waveDataParent;
    [SerializeField] TMP_InputField cooldownAtEndWave;
    [SerializeField] TMP_InputField cooldownBetweenEnemies;

    [Header("Enemy Wave Data UI")]
    [SerializeField] GameObject enemyInwaveUIPrefab;
    [SerializeField] Transform enemyInwaveUIContent;






    private void Start()
    {
        waveList = new List<WaveData>();

    }


    public void OnChangeWaveCount()
    {
        int listCount = 0;
        int.TryParse(waveCountInputText.text, out listCount);

        if(listCount == 0)
        {
            waveList.Clear();
        }
        else
        {
            if(listCount < waveList.Count)
            {
                int differenceAmount = waveList.Count - listCount;
                RemoveWaveElements(differenceAmount);
            }
            else
            {
                int differenceAmount = listCount - waveList.Count;

                AddWaveElements(differenceAmount);
            }
        }

        waveDataParent.gameObject.SetActive(false);
    }

    public void OnChangeWaveIndex()
    {
        int waveIndex = 0;
        int.TryParse(currentWaveIndex.text, out waveIndex);

        DisplayWaveAtIndex(waveIndex);
    }
    public void OnChangeWaveDelay()
    {
        float delay = 0;
        float.TryParse(cooldownAtEndWave.text, out delay);
        waveList[currentWaveSelectedIndex].delayBetweenWaves = delay;

    }
    public void OnChangeEnemiesDelay()
    {
        float delay = 0;
        float.TryParse(cooldownBetweenEnemies.text, out delay);
        waveList[currentWaveSelectedIndex].delayBetweenEnemies = delay;

    }
    public void AddEnemyToWave()
    {
        EnemyWaveData enemyData = new EnemyWaveData();

        GameObject go = Instantiate(enemyInwaveUIPrefab, enemyInwaveUIContent);
        
        if(go.TryGetComponent<EnemyInWaveUIData>(out EnemyInWaveUIData enemyInWaveData))
        {
            enemyInWaveData.InitEnemyInWaveUI(enemyData);
        }


        waveList[currentWaveSelectedIndex].enemyWaveDataList.Add(enemyData);
    }
    public void RemoveEnemyFromWave(EnemyWaveData data)
    {
        if (waveList[currentWaveSelectedIndex].enemyWaveDataList.Contains(data))
        {
            waveList[currentWaveSelectedIndex].enemyWaveDataList.Remove(data);
        }
    }

    private void DisplayWaveAtIndex(int index)
    {
        if(index >= waveList.Count)
        {
            ToolReferencerObject.Instance.toolUI.CallDisplaySystemMessage("Your wave index is incorrect, index stat from 0 and max is wave count -1");
            Debug.LogError("Error here");
            return;
        }

        waveDataParent.gameObject.SetActive(true);

        currentWaveSelectedIndex = index;

        cooldownAtEndWave.text = waveList[index].delayBetweenWaves.ToString();
        cooldownBetweenEnemies.text = waveList[index].delayBetweenEnemies.ToString();

        if(waveList[index].enemyWaveDataList == null || waveList[index].enemyWaveDataList.Count == 0)
        {
            waveList[index].enemyWaveDataList = new List<EnemyWaveData>();
        }

        foreach (Transform child in enemyInwaveUIContent)
        {
            Destroy(child.gameObject);
        }

        foreach (EnemyWaveData enemyWaveData in waveList[index].enemyWaveDataList)
        {
            GameObject go = Instantiate(enemyInwaveUIPrefab, enemyInwaveUIContent);

            if (go.TryGetComponent<EnemyInWaveUIData>(out EnemyInWaveUIData enemyInWaveData))
            {
                enemyInWaveData.InitEnemyInWaveUI(enemyWaveData);
            }

        }
    }

    public void ToggleWaveCreatorUI(bool isOn)
    {
        waveCreatorUI.SetActive(isOn);
    }


    [ContextMenu("Save now!")]
    public void SaveWaveAsNew()
    {

        WaveSO wave = new WaveSO();
        wave.waves = new List<WaveData>();

        foreach (WaveData waveData in waveList)
        {
            wave.waves.Add(waveData);
        }

        string path = "Assets/Avishy World/Scriptable Objects - Avishy/Waves/newSO.asset";
        AssetDatabase.CreateAsset(wave, path);
        AssetDatabase.SaveAssets();


        ClearOnSave();
    }

    private void ClearOnSave()
    {
        waveCountInputText.text = "0";
        currentWaveIndex.text = "0";
        cooldownAtEndWave.text = "0";
        cooldownBetweenEnemies.text = "0";

        foreach (Transform child in enemyInwaveUIContent)
        {
            Destroy(child.gameObject);
        }

        waveDataParent.gameObject.SetActive(false);
        waveList.Clear();
    }
    private void AddWaveElements(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            WaveData wave = new WaveData();

            waveList.Add(wave);
        }
    }
    private void RemoveWaveElements(int amount)
    {
        for (int i = amount; i > 0; i--)
        {
            waveList.RemoveAt(i);
        }
    }
}
