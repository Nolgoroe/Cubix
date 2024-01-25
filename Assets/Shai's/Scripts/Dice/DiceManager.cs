using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("Preset data")]
    [SerializeField] private List<DiceSO> startDice;
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

        if (Player.Instance.ReturnPlayerDice().Count > 0)
        {
            List<DieData> dieDataList = new List<DieData>();
            dieDataList.AddRange(Player.Instance.ReturnPlayerDice());

            for (int i = 0; i < dieDataList.Count; i++)
            {
                GameObject go = Instantiate(dieDataList[i].diePrefab, diceSlots[i].transform);

                go.TryGetComponent<Die>(out Die newDie);
                if (newDie)
                {
                    newDie.InitDiceInSlot(diceSlots[i].ReturnLockTransform(), dieDataList[i]);

                }
            }
        }
        else
        {
            for (int i = 0; i < startDice.Count; i++)
            {
                GameObject go = Instantiate(startDice[i].diePrefab, diceSlots[i].transform);

                go.TryGetComponent<Die>(out Die newDie);
                if (newDie)
                {
                    DieData data = CreateNewDieData(startDice[i]);

                    newDie.InitDiceInSlot(diceSlots[i].ReturnLockTransform(), data);

                }
            }
        }
    }

    private DieData CreateNewDieData(DiceSO diceSO)
    {
        //there is the exact same funcion in the shop manager - maybe create a static dice class

        DieData data = new DieData();

        data.dieType = diceSO.dieType;
        data.element = diceSO.element;
        data.material = diceSO.dieMaterial;

        List<DieFaceValue> tmpFaceValues = new List<DieFaceValue>();
        for (int i = 0; i < diceSO.resouceDataList.Count; i++)
        {
            DieFaceValue faceValue = new DieFaceValue(diceSO.resouceDataList[i], diceSO.buffDataList[i]);
            tmpFaceValues.Add(faceValue);
        }

        data.facesValues = tmpFaceValues;
        data.towerPrefabConnected = diceSO.towerPrefab;
        data.diePrefab = diceSO.diePrefab;

        Player.Instance.AddDieData(data);

        return data;
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
    public void RollResourcesAutomatic()
    {
        foreach (var roller in resourceDice)
        {
            roller.ReturnDieRoller().Roll();
        }
    }
    public void RollInWorldAutomatic()
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
        startDice.Add(die);
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
