using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceTypeCombo
{
    public ResourceType neededResource;
    public int amount;
}
public abstract class ItemShopParentSO : ScriptableObject
{
    [Header("Shop Required")]
    public Sprite sprite;
    public ResourceTypeCombo[] resourcesNeeded;
}
