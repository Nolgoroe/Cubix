using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//currently only give resource bundle
public class MysterySite : BaseSite
{
    [Header("Reward")]
    [SerializeField] private ResourceBundleSO bundleSO;

    [Header("UI")]
    [SerializeField] private float windowDuration;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private TMP_Text rewardTxt;

    [ContextMenu("yahoooo")]
    public override void LaunchSite()
    {
        if (Player.Instance)
        {
            string resourcesText = "";

            foreach (var item in bundleSO.resourcesToRecieve)
            {
                bundleSO.AddResources(item);
                resourcesText += "<br>+" + item.amount + " " + item.resource.ToString();
            }
            rewardTxt.text = "You Recieved" + resourcesText;
            StartCoroutine(DisplayRewardWindow());
        }
    }

    private IEnumerator DisplayRewardWindow()
    {
        rewardWindow.SetActive(true);
        yield return new WaitForSeconds(windowDuration);
        rewardWindow.SetActive(false);
    }

}
