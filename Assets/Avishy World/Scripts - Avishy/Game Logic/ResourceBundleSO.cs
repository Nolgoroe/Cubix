using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Bundle Creation", menuName = "ScriptableObjects/Resource Bundle")]
public class ResourceBundleSO : ItemShopParentSO
{
    [Header("To Buy")]
    public ResourceTypeCombo[] resourcesToRecieve;


    public void AddResources(ResourceTypeCombo combo)
    {
        Player.Instance.AddResourcesLive(combo.resource, combo.amount);
    }
}
