using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("References")]
    [SerializeField] private DieDataSpawner diceDataSpawner;

    [Header("Dice")]
    [SerializeField] private List<DiceSO> startDice;
    [SerializeField] private List<DieData> playerDice; // This becomes the main and ONLY reference to player dice after first level of game!

    [Header("Resource")]
    [SerializeField] private int iron;
    [SerializeField] private int energy;
    [SerializeField] private int lightning;
    [SerializeField] private int scrap;

    [Header("Health")]
    [SerializeField] int maxHP;
    [SerializeField] int currentPlayerHealth;

    [Header("Rerolls")]
    [SerializeField] private int rerollAmount;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (DiceSO diceSO in startDice)
        {
            diceDataSpawner.CreateNewDieData(diceSO);
        }
    }

    private void Start()
    {
        maxHP = 1000; //temp
        currentPlayerHealth = maxHP;
    }

    public void InitPlayer()
    {
        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, maxHP);

        UIManager.Instance.UpdateResources(iron, energy, lightning, scrap);
    }


    public void AddResourcesFromDice(Die die)
    {
        //maybe this needs a fix since all dice will call this function even if they are part of the world.
        if (die.ReturnInWorld()) return;


        DieFaceValue dieFaceVakue = die.GetTopValue();

        switch (dieFaceVakue.Resource.Type)
        {
            case ResourceType.Iron:
                iron += dieFaceVakue.Resource.Value;
                break;
            case ResourceType.Energy:
                energy += dieFaceVakue.Resource.Value;
                break;
            case ResourceType.Lightning:
                lightning += dieFaceVakue.Resource.Value;
                break;
            default:
                break;
        }

        UIManager.Instance.UpdateResources(iron, energy, lightning, scrap);
    }
    public bool AddRemoveScrap(int amount)
    {
        scrap += amount;
        UIManager.Instance.UpdateResources(iron, energy, lightning, scrap);
    
        if(scrap <= 0)
        {
            scrap = 0;
            return false;
        }

        return true;
    }


    public void RecieveRandomResource()
    {
        ResourceType[] myEnums = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        int randomResource = UnityEngine.Random.Range(0, myEnums.Length);

        AddResources(myEnums[randomResource], 10);

        UIManager.Instance.UpdateResources(iron, energy, lightning, scrap);
    }

    public void AddResources(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Iron:
                iron += amount;
                break;
            case ResourceType.Energy:
                energy += amount;
                break;
            case ResourceType.Lightning:
                lightning += amount;
                break;
            default:
                break;
        }
    }

    public int ReturnAmountOfResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Iron:
                return iron;
            case ResourceType.Energy:
                return energy;
            case ResourceType.Lightning:
                return lightning;
            default:
                return -1;
        }
    }
    public void RemoveResources(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Iron:
                iron -= amount;
                break;
            case ResourceType.Energy:
                energy -= amount;
                break;
            case ResourceType.Lightning:
                lightning -= amount;
                break;
            default:
                break;
        }
    }


    public void RecieveDMG(float amount) // this is here since this is the object that keeps track of player health between all scenes
    {
        currentPlayerHealth -= 1;

        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
        }

        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, maxHP);

        if (currentPlayerHealth <= 0)
        {
            Debug.Log("You have lost!");

            UIManager.Instance.DisplayEndGameScreen(false);
            GameManager.isDead = true;
            return;
        }
    }

    public void Heal(int amount)
    {
        currentPlayerHealth += amount;

        if(currentPlayerHealth > maxHP)
        {
            currentPlayerHealth = maxHP;
        }
    }
    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
    }

    public bool ReturnIsHurt()
    {
        return currentPlayerHealth < maxHP;
    }

    public void ChangeRerollAmount(int amount)
    {
        rerollAmount += amount;
    }
    public int ReturnRerollAmount()
    {
        return rerollAmount;
    }

    public void AddDieData(DieData dieData)
    {
        playerDice.Add(dieData);
    }
    public List<DieData> ReturnPlayerDice()
    {
        return playerDice;
    }
}
