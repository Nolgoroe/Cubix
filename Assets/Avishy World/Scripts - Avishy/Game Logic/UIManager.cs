using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text timerText;

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

    public void ChangeGameSpeed(int speed)
    {
        GameManager.Instance.gameSpeedTemp = speed;
    }
}
