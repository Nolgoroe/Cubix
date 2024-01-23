using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceTypeCombo
{
    public ResourceType resource;
    public int amount;
}
public abstract class ItemShopParentSO : ScriptableObject
{
    [Header("Shop Required")]
    public Sprite icon;
    public ResourceTypeCombo[] resourcesNeeded;
}
