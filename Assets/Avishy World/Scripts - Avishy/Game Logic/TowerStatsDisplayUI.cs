using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerStatsDisplayUI : MonoBehaviour
{
    [SerializeField] TMP_Text statText;
    public void SetText(string text)
    {
        statText.text = text;
    }
}
