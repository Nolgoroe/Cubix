using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] ItemShopParentSO connectedItemSO;
    [SerializeField] Button buyButton;
    [SerializeField] Image itemIcon;
    [SerializeField] Transform priceDisplayParent;
    [SerializeField] TMP_Text neededResourceUITextPrefab;
    [SerializeField] TMP_Text itemName;
    [SerializeField] List<TMP_Text> intantiatedTexsts;

    public void InitShopItem(Sprite icon, ItemShopParentSO item)
    {
        connectedItemSO = item;
        itemIcon.sprite = icon;
        itemName.text = item.itemName;

        for (int i = 0; i < item.resourcesNeeded.Length; i++)
        {
            TMP_Text priceText = Instantiate(neededResourceUITextPrefab, priceDisplayParent);

            int amountOfResourceHas = Player.Instance.ReturnAmountOfResource(item.resourcesNeeded[i].resource);
            string resourceName = item.resourcesNeeded[i].resource.ToString();
            priceText.text = amountOfResourceHas + "/" + item.resourcesNeeded[i].amount + $" {resourceName}";

            intantiatedTexsts.Add(priceText);
        }

        buyButton.onClick.AddListener(() => ShopManager.Instance.BuyItem(item));
    }

    public void RefreshPrices()
    {
        for (int i = 0; i < connectedItemSO.resourcesNeeded.Length; i++)
        {           
            int amountOfResourceHas = Player.Instance.ReturnAmountOfResource(connectedItemSO.resourcesNeeded[i].resource);
            string resourceName = connectedItemSO.resourcesNeeded[i].resource.ToString();
            intantiatedTexsts[i].text = amountOfResourceHas + "/" + connectedItemSO.resourcesNeeded[i].amount + $" {resourceName}";
        }
    }
}
