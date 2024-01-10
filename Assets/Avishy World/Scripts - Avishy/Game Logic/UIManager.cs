using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text towerText;

    private void Start()
    {
        Instance = this;
    }

    public void DisplayTimerText(bool show)
    {
        timerText.gameObject.SetActive(show);
    }

    public void SetWaveCountdownText(float time)
    {
        timerText.text = Mathf.Round(time).ToString();
    }
    public void SetTowerText(string text)
    {
        towerText.text = text;
    }
}
