using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStats
{
    maxHP,
    CurrentHP
}

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private List<Die> allDieInPlay;
    [SerializeField] private int iron;
    [SerializeField] private int energy;
    [SerializeField] private int lightning;
    [SerializeField] int playerHealth;
    [SerializeField] int currentPlayerHealth;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerHealth = 1000; //temp
        currentPlayerHealth = playerHealth;

        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, playerHealth);
    }

    public void ConnectPlayerAndDice()
    {
        allDieInPlay = new List<Die>();
        allDieInPlay.AddRange(FindObjectsOfType<Die>());

        foreach (Die die in allDieInPlay)
        {
            die.OnRollEndEvent.AddListener(AddResourcesAfterRoll);
        }
    }

    private void AddResourcesAfterRoll(Die die)
    {
        //maybe this needs a fix since all dice will all this function even if they are part of the world.
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

    public void RecieveRandomResource()
    {
        ResourceType[]  myEnums = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        int randomResource = UnityEngine.Random.Range(0, myEnums.Length);

        switch (myEnums[randomResource])
        {
            case ResourceType.Iron:
                iron += 10;
                break;
            case ResourceType.Energy:
                energy += 10;
                break;
            case ResourceType.Lightning:
                lightning += 10;
                break;
            default:
                break;
        }

        UIManager.Instance.UpdateResources(iron, energy, lightning);
    }


    public void RecieveDMG(float amount)
    {
        currentPlayerHealth -= 1;

        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
        }

        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, playerHealth);

        if (currentPlayerHealth <= 0)
        {
            Debug.Log("You have lost!");

            UIManager.Instance.DisplayEndGameScreen(false);
            GameManager.isDead = true;
            return;
        }
    }
}
