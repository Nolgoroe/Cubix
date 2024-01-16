using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceFaceDisplayUI : MonoBehaviour
{
    [SerializeField] Image connectedImage;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text textAmount;

    private void OnValidate()
    {
        connectedImage = GetComponent<Image>();
    }

    public void SetImage(DieFaceValue diceFaceValue, Die die, bool isResource)
    {
        if (isResource)
        {
            connectedImage.color = die.ReturnDiceColor();
            iconImage.sprite = diceFaceValue.Resource.Icon;
            textAmount.text = diceFaceValue.Resource.Value.ToString();
        }
        else
        {
            connectedImage.color = die.ReturnDiceColor() - Color.white * 0.2f; //temp
            iconImage.sprite = diceFaceValue.Buff.Icon;
            textAmount.text = diceFaceValue.Buff.Value.ToString();
        }
    }
}