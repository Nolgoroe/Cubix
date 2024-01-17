using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffHolderUI : MonoBehaviour
{
    [SerializeField] Image bgHolderImage;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text amountText;

    public void InitData(Color bgColor, Sprite iconSprite, string amountString)
    {
        bgHolderImage.color = bgColor;
        iconImage.sprite = iconSprite;
        amountText.text = amountString;
    }
}
