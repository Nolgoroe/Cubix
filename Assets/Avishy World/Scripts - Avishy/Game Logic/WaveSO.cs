using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveData
{
    public int numOfEnemies;
    public float delayBetweenEnemies;
    public float delayBetweenWaves;
    public GameObject enemyPrefab;
}

[CreateAssetMenu(fileName = "Wave Creation", menuName = "ScriptableObjects/Waves")]
public class WaveSO : ScriptableObject
{
    //this is temp for now - need data from adar about how the waves will be created adn what data they need
    public List<WaveData> waves;
}
