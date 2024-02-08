using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("References")]
    [SerializeField] private DieDataSpawner diceDataSpawner;
    [SerializeField] private MapLoader mapLoader;


    [Header("Dice")]
    [SerializeField] private List<DiceSO> startDice;
    [SerializeField] private List<DieData> playerDice; // This becomes the main and ONLY reference to player dice after first level of game!

    [Header("Resource")]
    [SerializeField] private int iron;
    [SerializeField] private int nova;
    [SerializeField] private int energy;
    [SerializeField] private int scrap;

    [Header("Health")]
    [SerializeField] int maxHP;
    [SerializeField] int currentPlayerHealth;

    [Header("Rerolls")]
    [SerializeField] private int rerollAmount;

    [Header("Rerolls")]
    [SerializeField] private List<Spells> unlockedSpells;

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

        UpdatePlayerUI();
    }

    public void UpdatePlayerUI()
    {
        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, maxHP);

        UIManager.Instance.UpdateResources(iron, nova, energy, scrap);
    }


    public void AddResourcesFromDice(Die die)
    {
        //maybe this needs a fix since all dice will call this function even if they are part of the world.
        if (die.ReturnInWorld()) return;

        DieFaceValue dieFaceValue = die.GetTopValue();

        AddResourcesLive(dieFaceValue.Resource.Type, dieFaceValue.Resource.Value);
    }
    public bool ReturnHasScrap()
    {
        return scrap > 0;
    }


    public void RecieveRandomResource()
    {
        ResourceType[] myEnums = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        int randomResource = UnityEngine.Random.Range(0, myEnums.Length - 1);

        AddResourcesLive(myEnums[randomResource], 10);
    }

    public void AddResourcesLive(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Iron:
                iron += amount;
                break;
            case ResourceType.Nova:
                nova += amount;
                break;
            case ResourceType.Energy:
                energy += amount;
                break;
            case ResourceType.scrap:
                scrap += amount;
                break;
            default:
                break;
        }

        if(amount > 0)
        SoundManager.Instance.PlaySoundOneShot(Sounds.RecieveResources);

        UIManager.Instance.UpdateResources(iron, nova, energy, scrap);

        UIManager.Instance.AddNewResourceToGive(resourceType, amount);
        //UIManager.Instance.InstantiateLootDisplayUI(resourceType, amount);

    }

    public int ReturnAmountOfResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Iron:
                return iron;
            case ResourceType.Nova:
                return nova;
            case ResourceType.Energy:
                return energy;
            case ResourceType.scrap:
                return scrap;
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
            case ResourceType.Nova:
                nova -= amount;
                break;
            case ResourceType.Energy:
                energy -= amount;
                break;
            case ResourceType.scrap:
                scrap -= amount;
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
    public List<Spells> ReturnUnlockedSpells()
    {
        return unlockedSpells;
    }
    public void AddToUnlockedSpells(Spells spell)
    {
        if(!unlockedSpells.Contains(spell))
        unlockedSpells.Add(spell);
    }

    public void SaveMapProgression(List<SiteNode> nodes)
    {
        mapLoader.SaveData(nodes);
    }
    
    public void LoadMapProgression(List<SiteNode> nodes)
    {
        mapLoader.LoadData(nodes);
    }
}
