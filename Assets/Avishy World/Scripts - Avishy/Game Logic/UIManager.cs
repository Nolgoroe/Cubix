using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[System.Serializable]
public class ResourceTypeToAmount
{
    public ResourceType resourceType;
    public int amount;
}
[System.Serializable]
public class ResourceTypeToAmountHolder
{
    public ResourceType resourceType;
    public List<ResourceTypeToAmount> resourceTypeToAmountList;
    public float timeDisplayLoot;
    public float currentTimeDisplayLoot;
}
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static bool menuOpened = false;

    [Header("Wave UI")]
    [SerializeField] private Transform winScreen;
    [SerializeField] private Transform loseScreen;

    [Header("Wave UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text waveCounterText;

    [Header("Tower UI")]
    [SerializeField] private TowerToolTipUI towerBuffDataHolderObject;
    [SerializeField] private TowerStatsDisplayUI towerStatsDisplayUI;
    [SerializeField] private Transform towerStatsDisplayScreen;
    [SerializeField] private Transform towerStatsDisplayUIParent;
    [SerializeField] TMP_Text towerNameText;

    [Header("HP UI")]
    [SerializeField] private TMP_Text playerHealthText;

    [Header("Player Area UI")]
    [SerializeField] private Transform diceFacesResourcesDisplayParent;
    [SerializeField] private Transform diceFacesBuffDisplayParent;

    [Header("Resources UI")]
    [SerializeField] private Transform ironParent;
    [SerializeField] private TMP_Text ironText;
    [SerializeField] private Transform energyParent;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private Transform lightningParent;
    [SerializeField] private TMP_Text lightningText;
    [SerializeField] private Transform scrapParent;
    [SerializeField] private TMP_Text scrapText;

    [Header("Prefabs")]
    [SerializeField] private DiceFaceDisplayUI diceFaceUIDisplayPreafb;

    [Header("Pause Menu")]
    [SerializeField] private Transform pauseMenu;

    [Header("Stamina")]
    [SerializeField] private TMP_Text staminaText;

    [Header("Map Dice")]
    [SerializeField] private DataDieDisplayUI diceDataDisplayUI;
    [SerializeField] private Transform diceDataDisplayParent;

    [Header("Scene Management")]
    [SerializeField] bool AddActionsToButtons;

    [Header("Loot Display UI")]
    [SerializeField] LootDisplayUI lootUIDisplayPrefab;
    [SerializeField] List<ResourceTypeToAmountHolder> resourceTypeToAmountHolderArray;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddActionToAllButotns();

        TogglePauseMenu(false);
    }

    private void Update()
    {
        foreach (ResourceTypeToAmountHolder holder in resourceTypeToAmountHolderArray)
        {
            if (holder.resourceTypeToAmountList.Count > 0)
            {
                holder.currentTimeDisplayLoot -= Time.deltaTime;

                if (holder.currentTimeDisplayLoot <= 0)
                {
                    holder.currentTimeDisplayLoot = holder.timeDisplayLoot;

                    InstantiateLootDisplayUI(holder.resourceTypeToAmountList[0].resourceType, holder.resourceTypeToAmountList[0].amount);
                    holder.resourceTypeToAmountList.RemoveAt(0);
                }
            }
        }
    }
    private void AddActionToAllButotns()
    {
        if (!AddActionsToButtons) return;

        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SoundManager.Instance.PlaySoundOneShot(Sounds.UIClick));
        }
    }

    public void InitUIManager()
    {
        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(false);


        //find all buttons and add the ui sound to on click.
    }

    public void DisplayTimerText(bool show)
    {
        timerText.gameObject.SetActive(show);
    }

    public void SetWaveCountdownText(float time)
    {
        if(time <= 5.5f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }

        timerText.text = Mathf.Round(time).ToString();
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
                GameObject resource = Instantiate(diceFaceUIDisplayPreafb.gameObject, diceFacesResourcesDisplayParent);
                GameObject buff = Instantiate(diceFaceUIDisplayPreafb.gameObject, diceFacesBuffDisplayParent);

                DiceFaceDisplayUI displayUIResource;
                DiceFaceDisplayUI displayUIBuff;

                resource.TryGetComponent<DiceFaceDisplayUI>(out displayUIResource);
                buff.TryGetComponent<DiceFaceDisplayUI>(out displayUIBuff);

                if (displayUIResource)
                {
                    displayUIResource.SetImage(face.GetFaceValue(), die, true);
                }

                if (displayUIBuff)
                {
                    displayUIBuff.SetImage(face.GetFaceValue(), die, false);
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
    public void DisplayDiceFacesUI(bool isDisplay, DieData dieData)
    {
        if(isDisplay)
        {
            foreach (DieFaceValue faceValue in dieData.facesValues)
            {
                GameObject resource = Instantiate(diceFaceUIDisplayPreafb.gameObject, diceFacesResourcesDisplayParent);
                GameObject buff = Instantiate(diceFaceUIDisplayPreafb.gameObject, diceFacesBuffDisplayParent);

                DiceFaceDisplayUI displayUIResource;
                DiceFaceDisplayUI displayUIBuff;

                resource.TryGetComponent<DiceFaceDisplayUI>(out displayUIResource);
                buff.TryGetComponent<DiceFaceDisplayUI>(out displayUIBuff);

                if (displayUIResource)
                {
                    displayUIResource.SetImage(faceValue, dieData, true);
                }

                if (displayUIBuff)
                {
                    displayUIBuff.SetImage(faceValue, dieData, false);
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

    public void UpdateResources(int _iron, int _energy, int _lightning, int _scrap)
    {
        ironText.text = _iron.ToString();
        energyText.text = _energy.ToString();
        lightningText.text = _lightning.ToString();
        scrapText.text = _scrap.ToString();
    }
    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        playerHealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void DisplayEndGameScreen(bool success)
    {
        winScreen.gameObject.SetActive(success ? true : false);
        loseScreen.gameObject.SetActive(success ? false : true);
    }
    
    public void UpdateWaveCounter()
    {
        int currentWave = WaveManager.Instance.ReturnCurrentWaveIndex() + 1;

        int maxWaves = WaveManager.Instance.ReturnWaveCount() - 1;
        waveCounterText.text = "Wave " + currentWave + "/" + maxWaves;
    }

    public void UpdateStaminaAmount(int amount)
    {
        staminaText.text = amount.ToString();
    }

    public void UpdateMapDiceDisplay()
    {
        foreach (DieData data in Player.Instance.ReturnPlayerDice())
        {
            DataDieDisplayUI dataDieDisplay = Instantiate(diceDataDisplayUI, diceDataDisplayParent);
            dataDieDisplay.InitDisplay(data);
        }
    }

    public void DisplayTowerStats(bool Display, TowerBaseParent towerData)
    {
        foreach (Transform child in towerStatsDisplayUIParent)
        {
            Destroy(child.gameObject);
        }

        if (Display)
        {
            foreach (string text in towerData.DisplayTowerStats())
            {
                TowerStatsDisplayUI display = Instantiate(towerStatsDisplayUI, towerStatsDisplayUIParent);

                display.SetText(text);
            }

            towerNameText.text = towerData.gameObject.name;

            towerStatsDisplayScreen.gameObject.SetActive(true);
        }
        else
        {
            towerStatsDisplayScreen.gameObject.SetActive(false);
        }
    }

    public void ToggleMapDieDisplay()
    {
        //called from button

        bool isActive = diceDataDisplayParent.gameObject.activeInHierarchy;

        diceDataDisplayParent.gameObject.SetActive(!isActive);
    }

    public void AddNewResourceToGive(ResourceType resourceType, int amount)
    {
        ResourceTypeToAmount newCombo = new ResourceTypeToAmount();
        newCombo.resourceType = resourceType;
        newCombo.amount = amount;

        ResourceTypeToAmountHolder foundItem = resourceTypeToAmountHolderArray.Where(x => x.resourceType == resourceType).FirstOrDefault();

        int index = (int)resourceType;

        if(foundItem != null)
        {
            index = resourceTypeToAmountHolderArray.IndexOf(foundItem, 0);
        }

        resourceTypeToAmountHolderArray[index].resourceTypeToAmountList.Add(newCombo);
    }
    public void InstantiateLootDisplayUI(ResourceType resourceType, int amount)
    {
        Color color = Color.white;
        Transform parent = null;

        if (amount > 0)
        {
            color = Color.green;
        }
        else
        {
            color = Color.red;
        }


        switch (resourceType)
        {
            case ResourceType.Iron:
                parent = ironParent;
                break;
            case ResourceType.Energy:
                parent = energyParent;
                break;
            case ResourceType.Lightning:
                parent = lightningParent;
                break;
            case ResourceType.scrap:
                parent = scrapParent;
                break;
            default:
                break;
        }


        string modifier = amount > 0 ? "+" : "-";

        Instantiate(lootUIDisplayPrefab, parent).SetTextUI(modifier + amount.ToString(), color);


    }
}
