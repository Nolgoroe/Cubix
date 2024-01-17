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

    private void Start() 
    {
        currentLevelEnemySpawners = GridManager.Instance.ReturnLevelEnemySpawners();
        currentIndexInWave = 0;

        
        timeForNextWave = waveSO.waves[currentIndexInWave].delayBetweenWaves / GameManager.gameSpeed;
        currentCountdown = timeForNextWave;

        //StartNextWave();
    }

    private void Update()
    {
        if (GameManager.gameSpeed == 0) return;

        if (!waveDone || levelComplete) return;
        
        if(/*currentCountdown <= 0 &&*/ !GameManager.playerTurn /*&& currentAmountOfEnemies == 0*/)
        {
            //StartNextWave();
        }

        //if(currentCountdown > 0 /*&& currentAmountOfEnemies == 0*/)
        //{
        //    currentCountdown -= Time.deltaTime * GameManager.gameSpeed;

        //    UIManager.Instance.SetWaveCountdownText(currentCountdown);
        //}
        //else
        //{
        //    //temp - this is called on update...
        //    UIManager.Instance.DisplayTimerText(false);
        //}
    }


    [ContextMenu("Start Next Wave")]
    public void StartNextWave()
    {
        //this might not need to be here when we have end of level logic. temp
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
        BeforeWaveStart();

        // this is the player's turn.
        yield return new WaitUntil(() => GameManager.playerTurn == false);

        //disable all spawner danger Icons
        foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        {
            spawnerCell.DisplayDangerIcon(false);
        }

        if (currentLevelEnemySpawners.Count > 0)
        {
            waveDone = false;

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


            //timeForNextWave = waveSO.waves[currentIndexInWave].delayBetweenWaves;
            //currentCountdown = timeForNextWave;
            //UIManager.Instance.DisplayTimerText(true);

            waveDone = true;

            currentIndexInWave++;

            if (currentIndexInWave > waveSO.waves.Count - 1)
            {
                Debug.Log("Finished all waves");
                levelComplete = true;
                yield break;
            }

            //from this point on it's the players turn.

            yield return new WaitUntil(() => currentAmountOfEnemies == 0);

            StartCoroutine(GameManager.Instance.SetPlayerTurn(true));

            AtWaveEnd();
        }
    }

    private void BeforeWaveStart()
    {
        foreach (MeleeTowerParentScript meleeTower in GameManager.Instance.ReturnMeleeTowersList())
        {
            meleeTower.CleanTroopsAtWaveStart();
        }
    }

    private void AtWaveEnd()
    {
        foreach (EnemySpawnerCell spawnerCell in currentLevelEnemySpawners)
        {
            spawnerCell.DisplayDangerIcon(false);
        }

        foreach (EnemyWaveData waveData in waveSO.waves[currentIndexInWave].enemyWaveDataList)
        {
            currentLevelEnemySpawners[waveData.enemySpawnerIndex].DisplayDangerIcon(true);
        }
    }


    public void ChangeEnemyCount(int amount)
    {
        currentAmountOfEnemies += amount;
    }
}
