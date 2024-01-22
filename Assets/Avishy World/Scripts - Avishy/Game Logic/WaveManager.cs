using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave data")]
    [SerializeField] private WaveSO waveSO;
    [SerializeField] private WaveSO currentWaveSO;
    [SerializeField] private int currentIndexInWave;

    [Header("Enemy data")]
    [SerializeField] private List<EnemySpawnerCell> currentLevelEnemySpawners;
    [SerializeField] private List<EnemyWaveData> enemiesForWave;
    [SerializeField] private int currentIndexOfEnemies;
    [SerializeField] private int currentAmountOfEnemies;
    [SerializeField] private float enemySpawnTime;
    [SerializeField] private float currentEnemySpawnTime;




    [SerializeField] private EnemySpawnerCell selectedSpawner;
    [SerializeField] private List<EnemySpawnerCell> selectedMultipleSpawners;

    [Header("Test data")]
    [SerializeField] private bool testRandomSpawners = false;
    [SerializeField] private bool waveDone = true;
    [SerializeField] private bool levelComplete = false;
    [SerializeField] private float timeForNextWave = 0;
    [SerializeField] private float currentCountdown = 0;
    private List<EnemySpawnerCell> tempSpawnersList;

    private void Awake()
    {
        Instance = this;
    }

    public void InitWaveManager() 
    {
        currentLevelEnemySpawners = GridManager.Instance.ReturnLevelEnemySpawners();
        currentIndexInWave = 0;

        
        timeForNextWave = waveSO.Waves[currentIndexInWave].delayBetweenWaves / GameManager.gameSpeed;
        currentCountdown = timeForNextWave;

        currentWaveSO = new WaveSO();
        currentWaveSO.Waves = new List<WaveData>();

        for (int i = 0; i < waveSO.Waves.Count; i++)
        {
            WaveData waveData = new WaveData();
            waveData.delayBetweenWaves = waveSO.Waves[i].delayBetweenWaves;
            waveData.delayBetweenEnemies = waveSO.Waves[i].delayBetweenEnemies;

            waveData.enemyWaveDataList = new List<EnemyWaveData>();

            if (waveSO.Waves[i].enemyWaveDataList.Count > 0)
            {
                for (int k = 0; k < waveSO.Waves[i].enemyWaveDataList.Count; k++)
                {
                    EnemyWaveData enemyWaveData = new EnemyWaveData();
                    enemyWaveData.amountOfThisEnemy = waveSO.Waves[i].enemyWaveDataList[k].amountOfThisEnemy;
                    enemyWaveData.enemySpawnerIndex = waveSO.Waves[i].enemyWaveDataList[k].enemySpawnerIndex;
                    enemyWaveData.enemyPathIndex = waveSO.Waves[i].enemyWaveDataList[k].enemyPathIndex;
                    enemyWaveData.enemyType = waveSO.Waves[i].enemyWaveDataList[k].enemyType;
                    
                    waveData.enemyWaveDataList.Add(enemyWaveData);
                }
            }

            currentWaveSO.Waves.Add(waveData);
        }

        if (testRandomSpawners)
        {
            DisplayRandomSpawnersDangerIcons();
        }
        else
        {
            foreach (EnemyWaveData waveData in waveSO.Waves[currentIndexInWave + 1].enemyWaveDataList)
            {
                currentLevelEnemySpawners[waveData.enemySpawnerIndex].DisplayDangerIcon(true);

            }
        }

        enemiesForWave = new List<EnemyWaveData>();

        UIManager.Instance.UpdateWaveCounter();
    }

    private void Update()
    {
        if (GameManager.gamePaused) return;

        if (!waveDone && currentAmountOfEnemies > 0)
        {
            CommenceWave();

            return;
        }

        if (!waveDone && currentAmountOfEnemies == 0)
        {
            AtWaveEnd();

            return;
        }

        if (!waveDone || levelComplete || currentAmountOfEnemies > 0 ) return;

        if (currentCountdown <= 0)
        {
            UIManager.Instance.DisplayTimerText(false);

            StartNextWave();
            return;
        }

        if (currentCountdown > 0)
        {
            currentCountdown -= Time.deltaTime * GameManager.gameSpeed;

            UIManager.Instance.SetWaveCountdownText(currentCountdown);

            UIManager.Instance.DisplayTimerText(true);
        }
    }


    [ContextMenu("Start Next Wave")]
    public void StartNextWave()
    {
        currentIndexInWave++;

        //this is reached when we try to start a wave but finished them all, meaning we won
        if (currentIndexInWave >= waveSO.Waves.Count)
        {
            Debug.Log("no more waves! weeeeee");
            levelComplete = true;
            UIManager.Instance.DisplayEndGameScreen(true);
            return;
        }

        BeforeWaveStart();

        CommenceWave();
    }

    private void CommenceWave()
    {
        if (GameManager.gameSpeed == 0) return;

        if (testRandomSpawners)
        {
            RandomSpawnEnemies();
        }
        else
        {
            PredeterminedSpawnEnemies();
        }
    }

    private void PredeterminedSpawnEnemies()
    {
        if (currentLevelEnemySpawners.Count > 0 && currentIndexOfEnemies < enemiesForWave.Count)
        {
            selectedSpawner = currentLevelEnemySpawners[enemiesForWave[currentIndexOfEnemies].enemySpawnerIndex];

            if (currentEnemySpawnTime < 0)
            {
                GameObject enemyToSpawn = GameManager.Instance.ReturnEnemyByType(enemiesForWave[currentIndexOfEnemies].enemyType);
                selectedSpawner.CallSpawnEnemy(enemyToSpawn, enemiesForWave[currentIndexOfEnemies].enemyPathIndex);

                ChangeEnemyCount(1); //add 1 to enemy count

                enemiesForWave[currentIndexOfEnemies].amountOfThisEnemy--;
                if (enemiesForWave[currentIndexOfEnemies].amountOfThisEnemy <= 0)
                {
                    currentIndexOfEnemies++;
                }

                currentEnemySpawnTime = enemySpawnTime;
            }
            else
            {
                currentEnemySpawnTime -= Time.deltaTime * GameManager.gameSpeed;
            }
        }
    }

    private void RandomSpawnEnemies()
    {
        if (selectedMultipleSpawners.Count > 0 && currentIndexOfEnemies < enemiesForWave.Count)
        {
            if (currentEnemySpawnTime < 0)
            {
                int randomSpawner = Random.Range(0, selectedMultipleSpawners.Count);
                GameObject enemyToSpawn = GameManager.Instance.ReturnEnemyByType(enemiesForWave[currentIndexOfEnemies].enemyType);

                int randomPath = Random.Range(0, selectedMultipleSpawners[randomSpawner].ReturnEnemyPathCellsList().Count);
                selectedMultipleSpawners[randomSpawner].CallSpawnEnemy(enemyToSpawn, randomPath);

                ChangeEnemyCount(1); //add 1 to enemy count

                enemiesForWave[currentIndexOfEnemies].amountOfThisEnemy--;
                if (enemiesForWave[currentIndexOfEnemies].amountOfThisEnemy <= 0)
                {
                    currentIndexOfEnemies++;
                }

                currentEnemySpawnTime = enemySpawnTime;
            }
            else
            {
                currentEnemySpawnTime -= Time.deltaTime * GameManager.gameSpeed;
            }
        }
    }

    private void BeforeWaveStart()
    {
        StartCoroutine(GameManager.Instance.SetPlayerTurn(false));

        foreach (MeleeTowerParentScript meleeTower in GameManager.Instance.ReturnMeleeTowersList())
        {
            meleeTower.CleanTroopsAtWaveStart();
        }

        //disable all spawner danger Icons
        foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        {
            spawnerCell.DisplayDangerIcon(false);
        }

        currentIndexOfEnemies = 0;

        enemiesForWave.Clear();
        enemiesForWave.AddRange(currentWaveSO.Waves[currentIndexInWave].enemyWaveDataList);

        enemySpawnTime = waveSO.Waves[currentIndexInWave].delayBetweenEnemies;
        currentEnemySpawnTime = -1;

        timeForNextWave = waveSO.Waves[currentIndexInWave].delayBetweenWaves;
        currentCountdown = timeForNextWave;

        waveDone = false;
    }

    private void AtWaveEnd()
    {
        foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        {
            spawnerCell.DisplayDangerIcon(false);
        }

        StartCoroutine(GameManager.Instance.SetPlayerTurn(true));


        waveDone = true;




        if (currentIndexInWave + 1 == waveSO.Waves.Count)
        {
            //No need to do after wave end if there is no next wave.
            return;
        }


        UIManager.Instance.UpdateWaveCounter();

        if (testRandomSpawners)
        {
            DisplayRandomSpawnersDangerIcons();
        }
        else
        {
            foreach (EnemyWaveData waveData in waveSO.Waves[currentIndexInWave].enemyWaveDataList)
            {
                currentLevelEnemySpawners[waveData.enemySpawnerIndex].DisplayDangerIcon(true);
            }
        }
    }

    private void DisplayRandomSpawnersDangerIcons()
    {
        selectedMultipleSpawners.Clear();
        tempSpawnersList = new List<EnemySpawnerCell>();

        int ramdomAmountOfSpawners = Random.Range(1, currentLevelEnemySpawners.Count + 1);

        tempSpawnersList.AddRange(currentLevelEnemySpawners);


        for (int i = 0; i < ramdomAmountOfSpawners; i++)
        {
            int randomSpawner = Random.Range(0, tempSpawnersList.Count);
            selectedMultipleSpawners.Add(tempSpawnersList[randomSpawner]);

            tempSpawnersList.RemoveAt(randomSpawner);
        }

        foreach (EnemySpawnerCell spawner in selectedMultipleSpawners)
        {
            spawner.DisplayDangerIcon(true);
        }
    }

    public void ChangeEnemyCount(int amount)
    {
        currentAmountOfEnemies += amount;
    }

    public void StartWaveEarly()
    {
        if (!waveDone || levelComplete) return;

        currentCountdown = 0;
        UIManager.Instance.DisplayTimerText(false);
        Player.Instance.RecieveRandomResource();
    }

    public int ReturnCurrentWaveIndex()
    {
        return currentIndexInWave;
    }

    public int ReturnWaveCount()
    {
        return waveSO.Waves.Count;
    }
}
