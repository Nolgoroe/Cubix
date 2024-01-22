using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice Creation", menuName = "ScriptableObjects/Dice")]
public class DiceSO : ItemShopParentSO
{
    [Header("Dice Related")]
    public TowerBaseParent towerPrefab;
    public Material dieMaterial;
    public DieType dieType;

    public List<ResourceData> resouceDataList;
    public List<BuffData> buffDataList;
}
