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
    [SerializeField] private Camera mainCamera; //temp
    [SerializeField] private Camera diceCamera; //temp

    [SerializeField] List<TowerBaseParent> allTowersPrefabs;
    [SerializeField] List<RangeTowerParentScript> summonedRangeTowers;
    [SerializeField] List<MeleeTowerParentScript> summonedMeleeTowers;

    public static bool playerTurn = true;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        gameSpeed = gameSpeedTemp;

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }




    public void CallSetPlayerTurn(bool isPlayerTurn)
    {
        //called from button
        StartCoroutine(SetPlayerTurn(isPlayerTurn));
    }
    public IEnumerator SetPlayerTurn(bool isPlayerTurn)
    {
        playerTurn = isPlayerTurn;

        if (isPlayerTurn)
        {
            foreach (TowerBaseParent tower in summonedRangeTowers)
            {
                tower.OnStartPlayerTurn();
            }

            foreach (TowerBaseParent tower in summonedMeleeTowers)
            {
                tower.OnStartPlayerTurn();
            }
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

        yield return new WaitForSeconds(1f); //temp
        DiceManager.Instance.RollInWorld();
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

    public List<MeleeTowerParentScript> ReturnMeleeTowersList()
    {
        return summonedMeleeTowers;
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
}
