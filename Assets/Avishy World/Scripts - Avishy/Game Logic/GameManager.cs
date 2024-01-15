using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static int gameSpeed = 1;
    
    public int gameSpeedTemp = 1;// Temp

    [SerializeField] private List<EnemyParent> allEnemies;
    [SerializeField] private Camera mainCamera; //temp
    [SerializeField] private Camera diceCamera; //temp

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        gameSpeed = gameSpeedTemp;
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
}
