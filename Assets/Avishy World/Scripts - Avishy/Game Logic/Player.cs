using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Dice")]
    [SerializeField] private List<Die> allDieInPlay; //used to write events to dice on thier spawn.

    [Header("Resource")]
    [SerializeField] private int iron;
    [SerializeField] private int energy;
    [SerializeField] private int lightning;

    [Header("Health")]
    [SerializeField] int maxHP;
    [SerializeField] int currentPlayerHealth;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        maxHP = 1000; //temp
        currentPlayerHealth = maxHP;

        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, maxHP);
    }
    private void AddResourcesAfterRoll(Die die)
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

        UIManager.Instance.UpdateResources(iron, energy, lightning);
    }





    public void ConnectPlayerAndDiceOnStartLevel()
    {
        allDieInPlay = new List<Die>();
        allDieInPlay.AddRange(FindObjectsOfType<Die>());

        foreach (Die die in allDieInPlay)
        {
            die.OnRollEndEvent.AddListener(AddResourcesAfterRoll);
        }
    }
    public void RecieveRandomResource()
    {
        ResourceType[] myEnums = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        int randomResource = UnityEngine.Random.Range(0, myEnums.Length);

        AddResources(myEnums[randomResource], 10);

        UIManager.Instance.UpdateResources(iron, energy, lightning);
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
}
