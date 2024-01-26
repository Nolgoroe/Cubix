using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    //public int numOfEnemies;
    public float delayBetweenEnemies;
    public float delayBetweenWaves;
    //public List<EnemyWaveData> enemyWaveDataList;
    public List<EnemyWaveCombo> enemyWaveCombo;
}
[System.Serializable]
public class EnemyWaveData
{
    public int amountOfThisEnemy;
    public int enemySpawnerIndex;
    public int enemyPathIndex;
    public EnemyTypes enemyType;
}
[System.Serializable]
public class EnemyWaveCombo
{
    public List<EnemyWaveData> enemyWaveDataList;
    public float timeToNextEnemies;
}

[CreateAssetMenu(fileName = "Wave Creation", menuName = "ScriptableObjects/Waves")]
public class WaveSO : ScriptableObject
{
    [SerializeField] private List<WaveData> waves;

    public List<WaveData> Waves { get { return waves; } set { waves = value; } }
}
