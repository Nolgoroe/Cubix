using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("Preset data")]
    [SerializeField] private List<DiceSlot> diceSlots;

    [Header("Live data")]
    [SerializeField] private List<Die> resourceDice;
    [SerializeField] private List<DieRoller> worldDice;
    [SerializeField] private List<Die> spellDice;


    private void Awake()
    {
        Instance = this;
    }

    public void InitDiceManager()
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


    public List<Die> ReturnResourceDice()
    {
        return resourceDice;
    }
}
