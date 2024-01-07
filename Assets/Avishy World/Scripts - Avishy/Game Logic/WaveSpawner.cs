using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveSO waveSO;
    [SerializeField] private List<EnemySpawnerCell> currentLevelEnemySpawners;
    [SerializeField] private int currentIndexInWave;

    EnemySpawnerCell selectedSpawner;
    private void Start() //temp
    {
        currentLevelEnemySpawners = GridManager.Instance.ReturnLevelEnemySpawners();
        currentIndexInWave = 0;
    }
    [ContextMenu("Start Next Wave")]
    private void ContextStartWave()
    {
        if (currentIndexInWave > waveSO.waves.Count - 1)
        {
            Debug.Log("You Won! no more waves! weeeeee");
            return;
        }

        StartCoroutine(StartWave());

    }

    private IEnumerator StartWave()
    {
        if(currentLevelEnemySpawners.Count > 0)
        {
            int randomNum = Random.Range(0, currentLevelEnemySpawners.Count);
            selectedSpawner = currentLevelEnemySpawners[randomNum];

            for (int i = 0; i < waveSO.waves[currentIndexInWave].numOfEnemies; i++)
            {
                yield return new WaitForSeconds(waveSO.waves[currentIndexInWave].delayBetweenEnemies);

                selectedSpawner.CallSpawnEnemy(waveSO.waves[currentIndexInWave].enemyPrefab);
            }

            currentIndexInWave++;
        }
    }
}
