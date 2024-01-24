using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("Preset data")]
    [SerializeField] private List<DiceSO> ownedDice;
    [SerializeField] private List<DiceSlot> diceSlots;

    [Header("Live data")]
    [SerializeField] private List<Die> resourceDice;
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

        for (int i = 0; i < ownedDice.Count; i++)
        {
            GameObject go = Instantiate(ownedDice[i].diePrefab, diceSlots[i].transform);
            go.TryGetComponent<Die>(out Die newDie);
            if(newDie)
            {
                newDie.InitDiceInSlot(diceSlots[i].ReturnLockTransform(), ownedDice[i]);
            }
        }

        //Player.Instance.ConnectPlayerAndDiceOnStartLevel();
    }

    public void RollResources()
    {
        if (Player.Instance.ReturnRerollAmount() <= 0) return;

        //called from button
        foreach (var roller in resourceDice)
        {
            roller.ReturnDieRoller().Roll();
        }

        Player.Instance.ChangeRerollAmount(-1);
        UIManager.Instance.UpdateStaminaAmount(Player.Instance.ReturnRerollAmount());
    }
    public void RollInWorld()
    {
        foreach (var roller in worldDice)
        {
            roller.Roll();
        }
    }



    public void AddDiceToResources(Die die)
    {
        if (!resourceDice.Contains(die))
        {
            resourceDice.Add(die);
        }
    }
    public void RemoveDiceToResources(Die die)
    {
        resourceDice.Remove(die);
    }

    public void AddDiceToWorld(DieRoller die)
    {
        if(!worldDice.Contains(die))
        {
            worldDice.Add(die);
        }
    }

    public void ResetDiceToWorldList()
    {
        worldDice.Clear();
    }

    public void AddDieFromShop(DiceSO die)
    {
        ownedDice.Add(die);
    }


    public Sprite ReturnIconByType(ResourceType resourceType)
    {
        return resourceTypeToIcon[resourceType];
    }
    public Sprite ReturnIconByType(BuffType resourceType)
    {
        return buffTypeToIcon[resourceType];
    }

    public List<Die> ReturnResourceDice()
    {
        return resourceDice;
    }
}
