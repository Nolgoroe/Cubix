using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiceTypeToPrefab
{
    public DiceSO diceSO;
    public GameObject dicePrefab;
}
public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("Preset data")]
    //[SerializeField] private List<DiceSO> allDiceSO;
    [SerializeField] private List<DiceTypeToPrefab> startingDice;
    [SerializeField] private List<DiceSlot> diceSlots;

    [Header("Live data")]
    [SerializeField] private List<DieRoller> resourceDice;
    [SerializeField] private List<DieRoller> worldDice;

    [Header("Dictionary data")]
    [SerializeField] private Sprite[] allResourceIcons;
    [SerializeField] private Sprite[] allBuffIcons;
    [SerializeField] private Dictionary<ResourceType, Sprite> resourceTypeToIcon;
    [SerializeField] private Dictionary<BuffType, Sprite> buffTypeToIcon;

    private void Awake()
    {
        Instance = this;
    }

    public void InitDiceManager()
    {
        resourceTypeToIcon = new Dictionary<ResourceType, Sprite>();
        buffTypeToIcon = new Dictionary<BuffType, Sprite>();

        for (int i = 0; i < allResourceIcons.Length; i++)
        {
            resourceTypeToIcon.Add((ResourceType)i, allResourceIcons[i]);
        }

        for (int i = 0; i < allBuffIcons.Length; i++)
        {
            buffTypeToIcon.Add((BuffType)i, allBuffIcons[i]);
        }

        for (int i = 0; i < startingDice.Count; i++)
        {
            GameObject go = Instantiate(startingDice[i].dicePrefab, diceSlots[i].transform);
            go.TryGetComponent<Die>(out Die newDie);
            if(newDie)
            {
                newDie.InitDiceInSlot(diceSlots[i].ReturnLockTransform(), startingDice[i].diceSO);
            }
        }

        Player.Instance.ConnectPlayerAndDice();
    }

    public void RollResources()
    {
        foreach (var roller in resourceDice)
        {
            roller.Roll();
        }
    }
    public void RollInWorld()
    {
        foreach (var roller in worldDice)
        {
            roller.Roll();
        }
    }

    //public Color returnRandomDiceColor()
    //{
    //    int random = Random.Range(0, diceColors.Count);
    //    return diceColors[random];
    //}

    //the next 4 functions are temp???? - try to find better way
    public void AddDiceToResources(DieRoller die) //temp
    {
        if (!resourceDice.Contains(die))
        {
            resourceDice.Add(die);
        }
    }
    public void RemoveDiceToResources(DieRoller die) //temp
    {
        resourceDice.Remove(die);
    }

    public void AddDiceToWorld(DieRoller die) //temp
    {
        if(!worldDice.Contains(die))
        {
            worldDice.Add(die);
        }
    }
    public void RemoveDiceToWorld(DieRoller die) //temp
    {
        worldDice.Remove(die);
    }
    public void ResetDiceToWorld() //temp
    {
        worldDice.Clear();
    }

    public Sprite ReturnIconByType(ResourceType resourceType)
    {
        return resourceTypeToIcon[resourceType];
    }
    public Sprite ReturnIconByType(BuffType resourceType)
    {
        return buffTypeToIcon[resourceType];
    }
}
