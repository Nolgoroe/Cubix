using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    //public int numOfEnemies;
    public float delayBetweenEnemies;
    public float delayBetweenWaves;
    public List<EnemyWaveData> enemyWaveData;
}
[System.Serializable]
public class EnemyWaveData
{
    public int amountOfThisEnemy;
    public int enemySpawnerIndex;
    public int enemyPathIndex;
    public EnemyTypes enemyType;
}

[CreateAssetMenu(fileName = "Wave Creation", menuName = "ScriptableObjects/Waves")]
public class WaveSO : ScriptableObject
{
    //this is temp for now - need data from adar about how the waves will be created adn what data they need
    public List<WaveData> waves;
}
