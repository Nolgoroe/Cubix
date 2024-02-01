using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealingSite : BaseSite
{
    [Header("Healing")]
    [SerializeField] private int healAmount;

    [Header("UI")]
    [SerializeField] private int popDuration;
    [SerializeField] private GameObject popWindow;
    [SerializeField] private TMP_Text popTxt;

    [ContextMenu("aggggggg")]
    public override void LaunchSite()
    {
        if (Player.Instance)
        {
            Player.Instance.Heal(healAmount);
            popTxt.text = "Healed for " + healAmount;
            StartCoroutine(DisplayPopWindow());
        }
    }

    private IEnumerator DisplayPopWindow()
    {
        popWindow.SetActive(true);
        yield return new WaitForSeconds(popDuration);
        popWindow.SetActive(false);
    }

}
