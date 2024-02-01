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
    [SerializeField] private bool waveDone = true;
    [SerializeField] private bool levelComplete = false;
    [SerializeField] private float timeForNextWave = 0;
    [SerializeField] private float currentCountdown = 0;

    [Header("Enemy data")]
    [SerializeField] private List<EnemySpawnerCell> currentLevelEnemySpawners;
    [SerializeField] private List<EnemyWaveCombo> currentEnemiesCombo; 
    //[SerializeField] private int currentIndexOfEnemiesCombo;
    [SerializeField] private int currentIndexInCombo; 
    [SerializeField] private int currentAmountOfEnemies;
    [SerializeField] private int currentAmountOfEnemiesForCombo;
    [SerializeField] private float enemySpawnTime; 
    [SerializeField] private float currentEnemySpawnTime; 



    [Header("Normal Wave Spawn")]
    [SerializeField] private EnemySpawnerCell selectedSpawner;

    [Header("Random Wave Spawn")]
    [SerializeField] private List<EnemySpawnerCell> selectedMultipleSpawners;
    [SerializeField] private bool testRandomSpawners = false;
    private List<EnemySpawnerCell> tempSpawnersList;





    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundFade(Sounds.PlannigPhaseBGM);
    }

    private void Update()
    {
        if (GameManager.gamePaused) return;

        if ((!waveDone && (currentAmountOfEnemies > 0 || currentAmountOfEnemiesForCombo > 0)))
        {
            CommenceWave();

            return;
        }

        if (!waveDone && currentAmountOfEnemies == 0 && currentAmountOfEnemiesForCombo == 0)
        {
            AtWaveEnd();

            return;
        }

        if (!waveDone || levelComplete || currentAmountOfEnemiesForCombo > 0 ) return;


        if (currentCountdown <= 0)
        {
            SoundManager.Instance.StopSound(Sounds.TimerTicking);
            UIManager.Instance.DisplayTimerText(false);

            StartNextWave();
            return;
        }

        if (currentCountdown > 0)
        {
            currentCountdown -= Time.deltaTime * GameManager.gameSpeed;

            if (currentCountdown <= 5.5f)
            {
                SoundManager.Instance.PlaySoundIfInactive(Sounds.TimerTicking);
            }

            UIManager.Instance.SetWaveCountdownText(currentCountdown);

            UIManager.Instance.DisplayTimerText(true);
        }
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
        if (currentLevelEnemySpawners.Count > 0 && currentIndexInCombo < currentEnemiesCombo.Count)
        {
            if (currentEnemySpawnTime < 0)
            {
                currentEnemySpawnTime = enemySpawnTime;

                bool enemiesAvailable = false;

                foreach (EnemyWaveData enemyData in currentEnemiesCombo[currentIndexInCombo].enemyWaveDataList)
                {
                    if (enemyData.amountOfThisEnemy <= 0)  continue;

                    GameObject enemyToSpawn = GameManager.Instance.ReturnEnemyByType(enemyData.enemyType);

                    selectedSpawner = currentLevelEnemySpawners[enemyData.enemySpawnerIndex];
                    selectedSpawner.CallSpawnEnemy(enemyToSpawn, enemyData.enemyPathIndex);

                    ChangeEnemyCount(1); //add 1 to enemy count
                    currentAmountOfEnemiesForCombo--;

                    enemyData.amountOfThisEnemy--;

                    currentLevelEnemySpawners[enemyData.enemySpawnerIndex].ChangeTowerEnemyText(1);

                    enemiesAvailable = true;
                }

                if(!enemiesAvailable)
                {
                    currentEnemySpawnTime = currentEnemiesCombo[currentIndexInCombo].timeToNextEnemies;

                    //combo is done, move to next combo if available.
                    currentIndexInCombo++;
                    // if currentIndexInCombo will be equal or  greater than the count of combo's, we won't summon anymore.

                }
            }

            currentEnemySpawnTime -= Time.deltaTime * GameManager.gameSpeed;
        }
    }

    private void RandomSpawnEnemies()
    {
        if (currentLevelEnemySpawners.Count > 0 && currentIndexInCombo < currentEnemiesCombo.Count)
        {
            if (currentEnemySpawnTime < 0)
            {
                currentEnemySpawnTime = enemySpawnTime;

                bool enemiesAvailable = false;

                foreach (EnemyWaveData enemyData in currentEnemiesCombo[currentIndexInCombo].enemyWaveDataList)
                {
                    if (enemyData.amountOfThisEnemy <= 0) continue;

                    GameObject enemyToSpawn = GameManager.Instance.ReturnEnemyByType(enemyData.enemyType);

                    int randomSpawner = Random.Range(0, selectedMultipleSpawners.Count);

                    selectedSpawner = selectedMultipleSpawners[randomSpawner];

                    int randomPath = Random.Range(0, selectedSpawner.ReturnEnemyPathCellsList().Count);
                    selectedSpawner.CallSpawnEnemy(enemyToSpawn, randomPath);

                    ChangeEnemyCount(1); //add 1 to enemy count
                    currentAmountOfEnemiesForCombo--;

                    enemyData.amountOfThisEnemy--;

                    enemiesAvailable = true;
                }

                if (!enemiesAvailable)
                {
                    currentEnemySpawnTime = currentEnemiesCombo[currentIndexInCombo].timeToNextEnemies;

                    //combo is done, move to next combo if available.
                    currentIndexInCombo++;
                    // if currentIndexInCombo will be equal or  greater than the count of combo's, we won't summon anymore.

                }
            }

            currentEnemySpawnTime -= Time.deltaTime * GameManager.gameSpeed;
        }
    }

    private void BeforeWaveStart()
    {
        StartCoroutine(GameManager.Instance.SetPlayerTurn(false));

        foreach (MeleeTowerParentScript meleeTower in GameManager.Instance.ReturnMeleeTowersList())
        {
            meleeTower.CleanTroopsAtWaveStart();
        }

        ////disable all spawner danger Icons
        //foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        //{
        //    spawnerCell.DisplayTowerIcons(false, 0);
        //}

        currentIndexInCombo = 0;
        currentAmountOfEnemiesForCombo = 0;

        currentEnemiesCombo.Clear();

        foreach (EnemyWaveCombo combo in currentWaveSO.Waves[currentIndexInWave].enemyWaveCombo)
        {
            currentEnemiesCombo.Add(combo);

            for (int i = 0; i < combo.enemyWaveDataList.Count; i++)
            {
                currentAmountOfEnemiesForCombo += combo.enemyWaveDataList[i].amountOfThisEnemy;
            }
        }

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
            spawnerCell.DisplayTowerIcons(false, 0);
        }

        foreach (Die die in DiceManager.Instance.ReturnResourceDice())
        {
            Player.Instance.AddResourcesFromDice(die);
        }


        waveDone = true;

        if (currentIndexInWave + 1 == waveSO.Waves.Count)
        {
            //No need to do after wave end if there is no next wave.
            return;
        }

        StartCoroutine(GameManager.Instance.SetPlayerTurn(true));

        UIManager.Instance.UpdateWaveCounter();

        if (testRandomSpawners)
        {
            DisplayRandomSpawnersDangerIcons();
        }
        else
        {
            foreach (EnemyWaveCombo waveCombo in waveSO.Waves[currentIndexInWave + 1].enemyWaveCombo)
            {
                for (int i = 0; i < waveCombo.enemyWaveDataList.Count; i++)
                {
                    currentLevelEnemySpawners[waveCombo.enemyWaveDataList[i].enemySpawnerIndex].DisplayTowerIcons(true, waveCombo.enemyWaveDataList[i].amountOfThisEnemy);
                }
            }

            currentEnemiesCombo.Clear();

            foreach (EnemyWaveCombo combo in currentWaveSO.Waves[currentIndexInWave].enemyWaveCombo)
            {
                currentEnemiesCombo.Add(combo);

                for (int i = 0; i < combo.enemyWaveDataList.Count; i++)
                {
                    currentAmountOfEnemiesForCombo += combo.enemyWaveDataList[i].amountOfThisEnemy;
                }
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
            spawner.DisplayTowerIcons(true, 0);
        }
    }





    public void InitWaveManager()
    {
        currentLevelEnemySpawners = GridManager.Instance.ReturnLevelEnemySpawners();
        currentIndexInWave = 0;


        timeForNextWave = waveSO.Waves[currentIndexInWave].delayBetweenWaves;
        currentCountdown = timeForNextWave;

        //currentWaveSO = new WaveSO();
        currentWaveSO = ScriptableObject.CreateInstance<WaveSO>();
        currentWaveSO.Waves = new List<WaveData>();

        for (int i = 0; i < waveSO.Waves.Count; i++)
        {
            WaveData waveData = new WaveData();
            waveData.delayBetweenWaves = waveSO.Waves[i].delayBetweenWaves;
            waveData.delayBetweenEnemies = waveSO.Waves[i].delayBetweenEnemies;

            waveData.enemyWaveCombo = new List<EnemyWaveCombo>();

            if (waveSO.Waves[i].enemyWaveCombo.Count > 0)
            {
                for (int k = 0; k < waveSO.Waves[i].enemyWaveCombo.Count; k++)
                {
                    EnemyWaveCombo combo = new EnemyWaveCombo();
                    combo.timeToNextEnemies = waveSO.Waves[i].enemyWaveCombo[k].timeToNextEnemies;


                    combo.enemyWaveDataList = new List<EnemyWaveData>();

                    for (int c = 0; c < waveSO.Waves[i].enemyWaveCombo[k].enemyWaveDataList.Count; c++)
                    {
                        EnemyWaveData enemyWaveData = new EnemyWaveData();
                        enemyWaveData.amountOfThisEnemy = waveSO.Waves[i].enemyWaveCombo[k].enemyWaveDataList[c].amountOfThisEnemy;
                        enemyWaveData.enemySpawnerIndex = waveSO.Waves[i].enemyWaveCombo[k].enemyWaveDataList[c].enemySpawnerIndex;
                        enemyWaveData.enemyPathIndex = waveSO.Waves[i].enemyWaveCombo[k].enemyWaveDataList[c].enemyPathIndex;
                        enemyWaveData.enemyType = waveSO.Waves[i].enemyWaveCombo[k].enemyWaveDataList[c].enemyType;

                        combo.enemyWaveDataList.Add(enemyWaveData);
                    }

                    waveData.enemyWaveCombo.Add(combo);
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
            foreach (EnemyWaveCombo waveCombo in waveSO.Waves[currentIndexInWave + 1].enemyWaveCombo)
            {
                for (int i = 0; i < waveCombo.enemyWaveDataList.Count; i++)
                {
                    currentLevelEnemySpawners[waveCombo.enemyWaveDataList[i].enemySpawnerIndex].DisplayTowerIcons(true, waveCombo.enemyWaveDataList[i].amountOfThisEnemy);
                }
            }
        }

        currentEnemiesCombo = new List<EnemyWaveCombo>();

        UIManager.Instance.UpdateWaveCounter();
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

            StartCoroutine(GameManager.Instance.BackToMap());
            return;
        }

        SoundManager.Instance.PlaySoundOneShot(Sounds.WaveStart);

        BeforeWaveStart();

        CommenceWave();
    }

    public void ChangeEnemyCount(int amount)
    {
        currentAmountOfEnemies += amount;

    }

    public void StartWaveEarly()
    {
        // called from button
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
