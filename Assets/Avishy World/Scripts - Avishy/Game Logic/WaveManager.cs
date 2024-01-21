using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [SerializeField] private WaveSO waveSO;
    [SerializeField] private List<EnemySpawnerCell> currentLevelEnemySpawners;
    [SerializeField] private int currentIndexInWave;
    [SerializeField] private int currentAmountOfEnemies;



    private EnemySpawnerCell selectedSpawner;
    private bool waveDone = false;
    private bool levelComplete = false;
    private float timeForNextWave = 0;
    private float currentCountdown = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void InitWaveManager() 
    {
        currentLevelEnemySpawners = GridManager.Instance.ReturnLevelEnemySpawners();
        currentIndexInWave = 0;

        
        timeForNextWave = waveSO.waves[currentIndexInWave].delayBetweenWaves / GameManager.gameSpeed;
        currentCountdown = timeForNextWave;
    }

    private void Start()
    {
        StartCoroutine(tempOnStart());
    }

    IEnumerator tempOnStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartNextWave();
    }
    private void Update()
    {
        if (GameManager.gamePaused) return;

        if (!waveDone || levelComplete) return;

        if (currentCountdown <= 0 && currentAmountOfEnemies == 0)
        {
            StartNextWave();
        }

        if (currentCountdown > 0 && currentAmountOfEnemies == 0)
        {
            currentCountdown -= Time.deltaTime * GameManager.gameSpeed;

            UIManager.Instance.SetWaveCountdownText(currentCountdown);

            //temp - this is called on update...
            UIManager.Instance.DisplayTimerText(true);
        }
        else
        {
            //temp - this is called on update...
            UIManager.Instance.DisplayTimerText(false);
        }
    }


    [ContextMenu("Start Next Wave")]
    public void StartNextWave()
    {
        //this is reached when we try to start a wave but finished them all, meaning we won
        if (currentIndexInWave >= waveSO.waves.Count)
        {
            Debug.Log("no more waves! weeeeee");
            levelComplete = true;
            UIManager.Instance.DisplayEndGameScreen(true);
            return;
        }


        StartCoroutine(StartWave());

    }

    private IEnumerator StartWave()
    {
        BeforeWaveStart();

        // this is the player's turn.
        //yield return new WaitUntil(() => GameManager.playerTurn == false);

        //disable all spawner danger Icons
        foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        {
            spawnerCell.DisplayDangerIcon(false);
        }

        if (currentLevelEnemySpawners.Count > 0)
        {
            foreach (EnemyWaveData enemyWaveData in waveSO.waves[currentIndexInWave].enemyWaveDataList)
            {
                selectedSpawner = currentLevelEnemySpawners[enemyWaveData.enemySpawnerIndex];

                for (int i = 0; i < enemyWaveData.amountOfThisEnemy; i++)
                {
                    while (GameManager.gameSpeed == 0)
                    {
                        yield return null;
                    }

                    GameObject enemyToSpawn = GameManager.Instance.ReturnEnemyByType(enemyWaveData.enemyType);
                    selectedSpawner.CallSpawnEnemy(enemyToSpawn);

                    ChangeEnemyCount(1);
                    yield return new WaitForSeconds(waveSO.waves[currentIndexInWave].delayBetweenEnemies / GameManager.gameSpeed);
                }
            }

            //This will need work.


            timeForNextWave = waveSO.waves[currentIndexInWave].delayBetweenWaves;
            currentCountdown = timeForNextWave;


            //from this point on it's the players turn.

            yield return new WaitUntil(() => currentAmountOfEnemies == 0);

            StartCoroutine(GameManager.Instance.SetPlayerTurn(true));

            AtWaveEnd();
        }
    }

    private void BeforeWaveStart()
    {

        

        waveDone = false;

        StartCoroutine(GameManager.Instance.SetPlayerTurn(false));

        foreach (MeleeTowerParentScript meleeTower in GameManager.Instance.ReturnMeleeTowersList())
        {
            meleeTower.CleanTroopsAtWaveStart();
        }
    }

    private void AtWaveEnd()
    {
        currentIndexInWave++;

        waveDone = true;

        foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        {
            spawnerCell.DisplayDangerIcon(false);
        }

        if (currentIndexInWave == waveSO.waves.Count)
        {
            //No need to do after wave end if there is no next wave.
            //UIManager.Instance.UpdateWaveCounter();

            return;
        }

        UIManager.Instance.UpdateWaveCounter();


        foreach (EnemyWaveData waveData in waveSO.waves[currentIndexInWave].enemyWaveDataList)
        {
            currentLevelEnemySpawners[waveData.enemySpawnerIndex].DisplayDangerIcon(true);
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
        return waveSO.waves.Count;
    }
}
