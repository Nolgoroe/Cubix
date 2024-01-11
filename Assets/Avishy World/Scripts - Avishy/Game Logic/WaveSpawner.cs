using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveSO waveSO;
    [SerializeField] private List<EnemySpawnerCell> currentLevelEnemySpawners;
    [SerializeField] private int currentIndexInWave;

    EnemySpawnerCell selectedSpawner;

    bool waveDone = false;
    bool levelComplete = false;
    float timeForNextWave = 0;
    float currentCountdown = 0;

    private void Start() 
    {
        currentLevelEnemySpawners = GridManager.Instance.ReturnLevelEnemySpawners();//temp
        currentIndexInWave = 0;//temp

        //temp
        timeForNextWave = waveSO.waves[currentIndexInWave].delayBetweenWaves / GameManager.gameSpeed;
        currentCountdown = timeForNextWave;

        StartNextWave();
    }

    private void Update()
    {
        if (GameManager.gameSpeed == 0) return;

        if (!waveDone || levelComplete) return;
        
        if(currentCountdown <= 0)
        {
            StartNextWave();
            UIManager.Instance.DisplayTimerText(false);
        }

        currentCountdown -= Time.deltaTime * GameManager.gameSpeed;

        UIManager.Instance.SetWaveCountdownText(currentCountdown);
    }


    [ContextMenu("Start Next Wave")]
    private void StartNextWave()
    {
        if (currentIndexInWave > waveSO.waves.Count - 1)
        {
            Debug.Log("no more waves! weeeeee");
            levelComplete = true;
            return;
        }

        StartCoroutine(StartWave());

    }

    private IEnumerator StartWave()
    {
        yield break;
        //if(currentLevelEnemySpawners.Count > 0)
        //{
        //    waveDone = false;

        //    int randomNum = Random.Range(0, currentLevelEnemySpawners.Count);
        //    selectedSpawner = currentLevelEnemySpawners[randomNum];

        //    //This will need work.

        //    for (int i = 0; i < waveSO.waves[currentIndexInWave].numOfEnemies; i++)
        //    {
        //        while (GameManager.gameSpeed == 0)
        //        {
        //            yield return null;
        //        }

        //        selectedSpawner.CallSpawnEnemy(waveSO.waves[currentIndexInWave].enemyWaveData[0].enemyPrefabs.gameObject);
        //        yield return new WaitForSeconds(waveSO.waves[currentIndexInWave].delayBetweenEnemies / GameManager.gameSpeed);
        //    }

        //    timeForNextWave = waveSO.waves[currentIndexInWave].delayBetweenWaves;
        //    currentCountdown = timeForNextWave;

        //    UIManager.Instance.DisplayTimerText(true);

        //    waveDone = true;

        //    currentIndexInWave++;
        //}
    }
}
