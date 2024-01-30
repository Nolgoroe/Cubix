using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LootDisplayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;

    private void Start()
    {
        Destroy(gameObject, 2);
    }
    public void SetTextUI(string text, Color color)
    {
        amountText.text = text;
        amountText.color = color;
    }
}
