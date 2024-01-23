using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice Creation", menuName = "ScriptableObjects/Dice")]
public class DiceSO : ItemShopParentSO
{
    [Header("Dice")]
    public GameObject diePrefab;
    public DieType dieType;
    public TowerBaseParent towerPrefab;
    public Material dieMaterial;

    [Header("Faces")]
    public List<ResourceData> resouceDataList;
    public List<BuffData> buffDataList;
}
