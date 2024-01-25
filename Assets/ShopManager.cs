using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField] ShopSO[] allShops;
    [SerializeField] ShopItemUI shopItemPrefab;
    [SerializeField] Transform shopItemsParent;
    [SerializeField] GameObject shopScreen;

    [SerializeField] List<ShopItemUI> instantiatedItems;

    [SerializeField] TMP_Text shopMessage;
    private string shopMessageString;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int randomShop = Random.Range(0, allShops.Length);

        for (int i = 0; i < allShops[randomShop].allShopItems.Length; i++)
        {
            ShopItemUI localItem = Instantiate(shopItemPrefab, shopItemsParent);

            ItemShopParentSO item = allShops[randomShop].allShopItems[i];
            Sprite icon = allShops[randomShop].allShopItems[i].icon;

            localItem.InitShopItem(icon, item);

            instantiatedItems.Add(localItem);
        }
    }

    public void BuyItem(ItemShopParentSO item)
    {
        shopMessageString = "";

        if (CheckConditionsToBuy(item))
        {
            foreach (ResourceTypeCombo neededCombo in item.resourcesNeeded)
            {
                Player.Instance.RemoveResources(neededCombo.resource, neededCombo.amount);
            }

            AddItemsToPlayer(item);
        }

        if(shopMessageString != "")
        {
            StopAllCoroutines();
            StartCoroutine(DisplayShopMessage());
        }

        UpdateAllPricesUI();
    }

    private void AddItemsToPlayer(ItemShopParentSO item)
    {
        switch (item)
        {
            case DiceSO die:
                DieData data = CreateNewDieData(die);
                break;
            case StatsBundleSO statsBundle:
                foreach (StatToAmoutCombo combo in statsBundle.statsToBuy)
                {
                    statsBundle.AddStat(combo);
                }

                break;
            case ResourceBundleSO resourceBundle:
                foreach (ResourceTypeCombo combo in resourceBundle.resourcesToRecieve)
                {
                    resourceBundle.AddResources(combo);
                }
                break;
            default:
                break;
        }
    }

    private bool CheckConditionsToBuy(ItemShopParentSO item)
    {
        if (!CheckConditionsToBuyItem(item)) return false;


        foreach (ResourceTypeCombo neededCombo in item.resourcesNeeded)
        {
            int amountOfResourceHas = Player.Instance.ReturnAmountOfResource(neededCombo.resource);
            
            if (amountOfResourceHas >= neededCombo.amount)
            {
                Debug.Log("OK");
            }
            else
            {
                Debug.Log($"Can't buy: + {item.ToString()}");
                shopMessageString = "Not enough resources to buy this.";
                return false;
            }
        }

        return true;
    }

    private bool CheckConditionsToBuyItem(ItemShopParentSO item)
    {
        switch (item)
        {
            case DiceSO die:
                return true; // no conditions
            case StatsBundleSO statsBundle:
                foreach (StatToAmoutCombo combo in statsBundle.statsToBuy)
                {
                    bool success = statsBundle.CheckCanAddStat(combo, out shopMessageString); // if we want to heal but are already at max HP, don't allow player to buy.

                    if (!success) return false;
                }

                return true;
            case ResourceBundleSO resourceBundle:
                return true; // no conditions
        }

        return false;
    }

    private void UpdateAllPricesUI()
    {
        foreach (ShopItemUI item in instantiatedItems)
        {
            item.RefreshPrices();
        }
    }

    private IEnumerator DisplayShopMessage()
    {
        shopMessage.text = shopMessageString;

        shopMessage.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);
        shopMessage.gameObject.SetActive(false);

    }


    private DieData CreateNewDieData(DiceSO diceSO)
    {
        //there is the exact same funcion in the Dice Manager - maybe create a static dice class

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
}
