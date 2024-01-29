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

    public static bool playerTurn = true;
    public static bool isDead = false;
    public static bool gamePaused = isDead || gameSpeed == 0;

    [Header("Cameras")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera diceCamera;

    [Header("Enemies")]
    [SerializeField] private List<EnemyParent> allEnemies;

    [Header("Towers")]
    [SerializeField] List<TowerBaseParent> allTowersPrefabs;
    [SerializeField] List<RangeTowerParentScript> summonedRangeTowers;
    [SerializeField] List<MeleeTowerParentScript> summonedMeleeTowers;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        WaveManager.Instance.InitWaveManager();
        UIManager.Instance.InitUIManager();
        DiceManager.Instance.InitDiceManager();
        Player.Instance.InitPlayer();

        UIManager.Instance.UpdateStaminaAmount(Player.Instance.ReturnRerollAmount());

        isDead = false;

        playerTurn = true;
    }

    private void Update()
    {
        gamePaused = isDead || gameSpeed == 0;

        gameSpeed = gameSpeedTemp;

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(2);
        }
    }






    public IEnumerator SetPlayerTurn(bool isPlayerTurn)
    {
        ChangeGameSpeed(1);

        playerTurn = isPlayerTurn;

        if (isPlayerTurn)
        {
            SoundManager.Instance.PlaySoundNormal(Sounds.TimerTicking);

            foreach (TowerBaseParent tower in summonedRangeTowers)
            {
                StartCoroutine(tower.OnStartPlayerTurn());
            }

            foreach (TowerBaseParent tower in summonedMeleeTowers)
            {
                StartCoroutine(tower.OnStartPlayerTurn());
            }

            SpellManager.Instance.CountdownAllSpells();

            DiceManager.Instance.RollResourcesAutomatic();
            yield return new WaitForSeconds(1f); //temp here to let dice fall into place before rolling
            DiceManager.Instance.RollInWorldAutomatic();

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
        // called from button
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
        //also called from button - has to be public
        gameSpeedTemp = speed;
    }

    public IEnumerator BackToMap()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}
