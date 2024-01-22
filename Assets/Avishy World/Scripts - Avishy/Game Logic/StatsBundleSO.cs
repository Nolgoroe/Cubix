using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Stats Bundle Creation", menuName = "ScriptableObjects/Stats Bundle")]
public class StatsBundleSO : ItemShopParentSO
{
    [Header("To Buy")]
    public PlayerStats playerStat;
    public float amount;
}
