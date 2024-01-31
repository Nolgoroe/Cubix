using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeUITemp : MonoBehaviour
{
    [SerializeField] private TMP_Text resourceTxt;

    public void UpdateResourceText()
    {
        resourceTxt.text = "Current Amount: " + Player.Instance?.ReturnAmountOfResource(ResourceType.Iron).ToString();
    }
}
