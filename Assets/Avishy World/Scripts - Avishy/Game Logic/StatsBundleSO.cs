using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum statsToBuyEnum
{
    maxHP,
    CurrentHP
}

[System.Serializable]
public class StatToAmoutCombo
{
    public statsToBuyEnum statToBuy;
    public int amount;
}
[CreateAssetMenu(fileName = "Stats Bundle Creation", menuName = "ScriptableObjects/Stats Bundle")]
public class StatsBundleSO : ItemShopParentSO
{
    [Header("To Buy")]
    public StatToAmoutCombo[] statsToBuy;


    public void AddStat(StatToAmoutCombo combo)
    {
        switch (combo.statToBuy)
        {
            case statsToBuyEnum.maxHP:
                Player.Instance.IncreaseMaxHP(combo.amount);
                break;
            case statsToBuyEnum.CurrentHP:
                Player.Instance.Heal(combo.amount);
                break;
            default:
                break;
        }
    }

    public bool CheckCanAddStat(StatToAmoutCombo combo, out string systemMessage)
    {
        systemMessage = "";
        switch (combo.statToBuy)
        {
            case statsToBuyEnum.maxHP:
                return true; // no condition
            case statsToBuyEnum.CurrentHP:
                bool success = Player.Instance.ReturnIsHurt(); // allow to heal only if missing health.
                if (!success)
                {
                    systemMessage = "Already at max health";
                }
                return success;
        }


        return false;
    }
}
