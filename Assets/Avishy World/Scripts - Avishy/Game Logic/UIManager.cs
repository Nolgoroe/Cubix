using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TowerToolTipUI towerBuffDataHolderObject;
    [SerializeField] private Transform diceFacesResourcesDisplayParent;
    [SerializeField] private Transform diceFacesBuffDisplayParent;
    [SerializeField] private DiceFaceDisplayUI diceFaceUiDisplayPreafb;

    [Header("Pause Menu")]
    [SerializeField] private Transform pauseMenu;


    public static bool menuOpened = false;

    private void Start()
    {
        Instance = this;

        TogglePauseMenu(false);
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

    public void DisplayTowerBuffData(bool isDisplay, TowerBaseParent tower)
    {
        if(isDisplay && !towerBuffDataHolderObject.gameObject.activeInHierarchy)
        {
            towerBuffDataHolderObject.gameObject.SetActive(true);
            towerBuffDataHolderObject.DisplayTowerBuffs(tower);
        }

        if(!isDisplay && towerBuffDataHolderObject.gameObject.activeInHierarchy)
        {
            towerBuffDataHolderObject.gameObject.SetActive(false);
        }
    }

    public void DisplayDiceFacesUI(bool isDisplay, Die die)
    {
        if(isDisplay)
        {
            foreach (DieFace face in die.GetAllFaces())
            {
                GameObject go = Instantiate(diceFaceUiDisplayPreafb.gameObject, diceFacesResourcesDisplayParent);
                DiceFaceDisplayUI displayUI;

                go.TryGetComponent<DiceFaceDisplayUI>(out displayUI);

                if(displayUI)
                {
                    displayUI.SetImage(face.GetFaceValue(), die, true);
                }
            }

            foreach (DieFace face in die.GetAllFaces())
            {
                GameObject go = Instantiate(diceFaceUiDisplayPreafb.gameObject, diceFacesBuffDisplayParent);
                DiceFaceDisplayUI displayUI;

                go.TryGetComponent<DiceFaceDisplayUI>(out displayUI);

                if(displayUI)
                {
                    displayUI.SetImage(face.GetFaceValue(), die, false);
                }
            }
        }
        else
        {
            foreach (Transform child in diceFacesResourcesDisplayParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in diceFacesBuffDisplayParent)
            {
                Destroy(child.gameObject);
            }
        }

        diceFacesResourcesDisplayParent.gameObject.SetActive(isDisplay);
        diceFacesBuffDisplayParent.gameObject.SetActive(isDisplay);
    }

    public void TogglePauseMenu(bool displayPause)
    {
        pauseMenu.gameObject.SetActive(displayPause ? true : false);

        menuOpened = displayPause;
    }

    public void GoToMainMenu()
    {
        Debug.Log("Go to main menu");
    }
    public void OpenSettings()
    {
        Debug.Log("Open Settings");
    }
}
