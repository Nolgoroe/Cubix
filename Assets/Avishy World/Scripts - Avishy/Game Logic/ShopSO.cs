using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Creation", menuName = "ScriptableObjects/Shop")]
public class ShopSO : ScriptableObject
{
    public ItemShopParentSO[] allShopItems;

}
