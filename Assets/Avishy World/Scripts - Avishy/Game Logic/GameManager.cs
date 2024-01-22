using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static int gameSpeed = 1;
    
    public int gameSpeedTemp = 1;// Temp

    [SerializeField] private List<EnemyParent> allEnemies;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera diceCamera;

    [SerializeField] List<TowerBaseParent> allTowersPrefabs;
    [SerializeField] List<RangeTowerParentScript> summonedRangeTowers;
    [SerializeField] List<MeleeTowerParentScript> summonedMeleeTowers;

    public static bool playerTurn = true;
    public static bool isDead = false;

    public static bool gamePaused = isDead || gameSpeed == 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        WaveManager.Instance.InitWaveManager();
        UIManager.Instance.InitUIManager();
        DiceManager.Instance.InitDiceManager();

        isDead = false;
    }

    private void Update()
    {
        gamePaused = isDead || gameSpeed == 0;

        gameSpeed = gameSpeedTemp;

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }




    public IEnumerator SetPlayerTurn(bool isPlayerTurn)
    {
        ChangeGameSpeed(1);

        playerTurn = isPlayerTurn;

        if (isPlayerTurn)
        {
            foreach (TowerBaseParent tower in summonedRangeTowers)
            {
                StartCoroutine(tower.OnStartPlayerTurn());
            }

            foreach (TowerBaseParent tower in summonedMeleeTowers)
            {
                StartCoroutine(tower.OnStartPlayerTurn());
            }

            yield return new WaitForSeconds(1f); //temp
            DiceManager.Instance.RollInWorld();
        }
        else
        {
            foreach (TowerBaseParent tower in summonedRangeTowers)
            {
                tower.OnEndPlayerTurn();
            }

            foreach (TowerBaseParent tower in summonedMeleeTowers)
            {
                tower.OnEndPlayerTurn();
            }
        }
    }

    public void AddTowerToRelaventList(TowerBaseParent tower)
    {
        switch (tower)
        {
            case RangeTowerParentScript rangeTower:
                summonedRangeTowers.Add(rangeTower);
                break;
            case MeleeTowerParentScript meleeTower:
                summonedMeleeTowers.Add(meleeTower);
                break;
            default:
                break;
        }
    }
    public void ClearTowerToRelaventList()
    {
        summonedRangeTowers.Clear();
        summonedMeleeTowers.Clear();
    }

    public List<MeleeTowerParentScript> ReturnMeleeTowersList()
    {
        return summonedMeleeTowers;
    }

    public Camera ReturnMainCamera()
    {
        return mainCamera;
    }
    public Camera ReturnDiceCamera()
    {
        return diceCamera;
    }

    public GameObject ReturnEnemyByType(EnemyTypes type)
    {
        EnemyParent enemy = allEnemies.Where(x => x.RetrunEnemyType() == type).FirstOrDefault();

        return enemy.gameObject;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    public void RestartRun()
    {
        Debug.Log("Restart game");
    }

    public TowerBaseParent ReturnRandomTowerPrefab()
    {
        int random = Random.Range(0, allTowersPrefabs.Count);
        return allTowersPrefabs[random];
    }

    public void ChangeGameSpeed(int speed)
    {
        gameSpeedTemp = speed;
    }
}
