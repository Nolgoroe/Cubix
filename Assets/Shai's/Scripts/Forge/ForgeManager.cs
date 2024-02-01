using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


//take all ui elements and logic into a new ui script
public class ForgeManager : MonoBehaviour
{
    [Header("Text Display")]
    [SerializeField] private TMP_Text cubeNameTxt;
    [SerializeField] private TMP_Text faceNumTxt;
    [SerializeField] private TMP_Text resourceTypeTxt;
    [SerializeField] private TMP_Text buffTypeTxt;
    [SerializeField] private TMP_Text resourceValueTxt;
    [SerializeField] private TMP_Text buffValueTxt;
    [SerializeField] private TMP_Text UpgradeDiePriceTxt;
    [SerializeField] private TMP_Text UpgradeFacePriceTxt;
    [SerializeField] private Button nextFaceButton;
    [SerializeField] private Button prevFaceButton;
    [SerializeField] private Button nextDieButton;
    [SerializeField] private Button prevDieButton;

    [Header("DieDisplay")]
    [SerializeField] private DisplayDicePair d6Die;
    [SerializeField] private DisplayDicePair d8Die;

    [Header("References")]
    [SerializeField] private BaseDiceDataSO baseDiceInfo;
    [SerializeField] private List<ForgeDieData> dice;
    [SerializeField] private GameObject blankD6Prefab;
    [SerializeField] private GameObject blankD8Prefab;
    [SerializeField] private ForgeUITemp forgeUI;
    [SerializeField] private Sprite[] D6Icons;
    [SerializeField] private Sprite[] D8Icons;

    private DisplayDicePair currentDisplayDice;

    private int currentDieIndex;
    private ResourceData _currentEditResource;
    private BuffData _currentEditBuff;

    [Header("Temp Settings")]//for standalone testing
    [SerializeField] private List<Die> realDice;
    [SerializeField] private int upgradeDiePrice;//TEMP!!!!!
    [SerializeField] private int upgradeFacePrice;//TEMP!!!!!


    private void Start()
    {
        Init(TempDieDataExtractor());//only for testing
        if (Player.Instance)
        {
            Init(Player.Instance.ReturnPlayerDice());
        }
        UpgradeDiePriceTxt.text = "Price: " + upgradeDiePrice;//TEMP!!!!!
        UpgradeFacePriceTxt.text = "Price: " + upgradeFacePrice;//TEMP!!!!!
        RefreshFaceNavButtons(dice[currentDieIndex]);
        RefreshDieNavButtons();
        forgeUI.UpdateResourceText();
    }

    private List<DieData> TempDieDataExtractor()
    {
        List<DieData> extractedData = new List<DieData>();
        foreach (var die in realDice)
        {
            extractedData.Add(die.ExportTransferData());
        }

        return extractedData;
    }

    public void Init(List<DieData> _diceData)
    {
        AssignDice(_diceData);
        UpdateCurrentDieView();
    }

    public void AssignDice(List<DieData> _diceData)
    {
        dice.Clear();

        //assign new dice
        foreach (var dieData in _diceData)
        {
            dice.Add(new ForgeDieData(dieData));
        }
    }

    public void SetDieIndex(int newIndex)
    {
        currentDieIndex = newIndex;
        UpdateCurrentDieView();
    }

    public void ChangeDieIndexByStep(int step)
    {
        currentDieIndex += step;
        currentDieIndex = Mathf.Clamp(currentDieIndex, 0, dice.Count - 1);
        RefreshDieNavButtons();
        UpdateCurrentDieView();
    }

    public void ChangeDieFaceIndexByStep(int step)
    {
        ForgeDieData currentforgeDie = dice[currentDieIndex];

        currentforgeDie.currentFaceindex += step;
        currentforgeDie.currentFaceindex = Mathf.Clamp(currentforgeDie.currentFaceindex, 0, currentforgeDie.dieData.facesValues.Count - 1);
        RefreshFaceNavButtons(currentforgeDie);
        UpdateCurrentDieView();
    }

    private void UpdateCurrentDieView()
    {
        ForgeDieData currentDie = dice[currentDieIndex];
        //temp text diplay, change it to actual die display later
        //cubeNameTxt.text = "Die: " + currentDie.die.name; 
        faceNumTxt.text = "Face " + (currentDie.currentFaceindex + 1).ToString();
        resourceTypeTxt.text = currentDie.GetCurrentFaceValue().Resource.Type.ToString();
        buffTypeTxt.text = currentDie.GetCurrentFaceValue().Buff.Type.ToString();
        resourceValueTxt.text = currentDie.GetCurrentFaceValue().Resource.Value.ToString();
        buffValueTxt.text = currentDie.GetCurrentFaceValue().Buff.Value.ToString();


        //dice display

        //determine wihch die model should be used
        switch (currentDie.dieData.dieType)
        {
            case DieType.D6:

                d8Die.buffDie.gameObject.SetActive(false);
                d8Die.resourceDie.gameObject.SetActive(false);

                d6Die.buffDie.gameObject.SetActive(true);
                d6Die.resourceDie.gameObject.SetActive(true);

                currentDisplayDice = d6Die;
                break;
            case DieType.D8:

                d8Die.buffDie.gameObject.SetActive(true);
                d8Die.resourceDie.gameObject.SetActive(true);

                d6Die.buffDie.gameObject.SetActive(false);
                d6Die.resourceDie.gameObject.SetActive(false);

                currentDisplayDice = d8Die;
                break;
            default:
                break;
        }

        forgeUI.UpdateResourceText();

        //update display buff die
        currentDisplayDice.buffDie.UpdateDisplay(
            currentDie.dieData.material,
            currentDie.GetCurrentFaceValue().Buff.Value.ToString() + "%",
            currentDie.GetCurrentFaceValue().Buff.Icon);

        //update display buff die
        currentDisplayDice.resourceDie.UpdateDisplay(
            currentDie.dieData.material,
            "+" + currentDie.GetCurrentFaceValue().Resource.Value.ToString(),
            currentDie.GetCurrentFaceValue().Resource.Icon);

    }

    public void ChangeCurrentFacePair()
    {
        SoundManager.Instance.PlaySoundOneShot(Sounds.ForgeDice);

        dice[currentDieIndex].GetCurrentFaceValue().SetResource(_currentEditResource);
        dice[currentDieIndex].GetCurrentFaceValue().SetBuff(_currentEditBuff);

        UpdateCurrentDieView();
    }

    public void ChangeCurrentFaceResource()
    {
        dice[currentDieIndex].GetCurrentFaceValue().SetResource(_currentEditResource);
        UpdateCurrentDieView();

        //dice[currentDieIndex].die.DisplayResources();
    }

    public void ChangeCurrentFaceBuff()
    {
        dice[currentDieIndex].GetCurrentFaceValue().SetBuff(_currentEditBuff);
        UpdateCurrentDieView();


        //dice[currentDieIndex].die.DisplayBuffs();
    }

    public void UpgradeCurrentDieFace()
    {
        if (Player.Instance)//***SUPER TEMP****
        {
            //if scrap is currency
            //Player.Instance.AddRemoveScrap(-upgradeFacePrice);

            //if else
            Player.Instance.RemoveResources(ResourceType.Iron, -upgradeFacePrice);
        }

        SoundManager.Instance.PlaySoundOneShot(Sounds.ForgeDice);

        dice[currentDieIndex].GetCurrentFaceValue().UpgradeFace(2, 2);
        UpdateCurrentDieView();
    }

    public void SetForgeCurrentEditFacePair(DieFaceValue faceValue)
    {
        _currentEditResource = faceValue.Resource;
        _currentEditBuff = faceValue.Buff;
    }

    public void SetForgeCurrentEditResource(ResourceData resource)
    {
        _currentEditResource = resource;
    }

    public void SetForgeCurrentEditBuff(BuffData buff)
    {
        _currentEditBuff = buff;
    }


    //increase number of faces
    public void UpgradeCurrentDie()
    {
        switch (dice[currentDieIndex].dieData.dieType)
        {
            case DieType.D6:
                //add two faces, might want to change the logic
                AddFacesToDie(2);

                if (Player.Instance)//***SUPER TEMP****
                {
                    //if scrap is currency
                    //Player.Instance.AddRemoveScrap(-upgradeDiePrice);

                    //if else
                    Player.Instance.RemoveResources(ResourceType.Iron, -upgradeDiePrice);
                }

                dice[currentDieIndex].dieData.dieType = DieType.D8;
                dice[currentDieIndex].dieData.diePrefab = blankD8Prefab;
                dice[currentDieIndex].dieData.dieIcon = D8Icons[(int)dice[currentDieIndex].dieData.colorType];
                
                
                break;
            case DieType.D8:
                Debug.Log("Still dont have upgrade for d8");
                break;
            default:
                break;
        }
        SoundManager.Instance.PlaySoundOneShot(Sounds.ForgeDice);

        UpdateCurrentDieView();
    }

    public void AddFacesToDie(int facesAmount)
    {
        DieData currentDieData = dice[currentDieIndex].dieData;
        for (int i = 0; i < facesAmount; i++)
        {
            DieFaceValue newFace;
            newFace = baseDiceInfo.GetBaseFaceValueOfDie(currentDieData.colorType, currentDieData.dieType, currentDieData.facesValues.Count);
            currentDieData.facesValues.Add(newFace);
        }
    }

    private void RefreshFaceNavButtons(ForgeDieData currentforgeDie)
    {
        //from here
        if (currentforgeDie.currentFaceindex >= currentforgeDie.dieData.facesValues.Count - 1)
        {
            nextFaceButton.gameObject.SetActive(false);
        }
        else
        {
            nextFaceButton.gameObject.SetActive(true);
        }

        if (currentforgeDie.currentFaceindex == 0)
        {
            prevFaceButton.gameObject.SetActive(false);
        }
        else
        {
            prevFaceButton.gameObject.SetActive(true);
        }
        //to here: trash. please do this better without ifs. or maybe not?
    }

    private void RefreshDieNavButtons()
    {
        //from here
        if (currentDieIndex >= dice.Count - 1)
        {
            nextDieButton.gameObject.SetActive(false);
        }
        else
        {
            nextDieButton.gameObject.SetActive(true);
        }

        if (currentDieIndex == 0)
        {
            prevDieButton.gameObject.SetActive(false);
        }
        else
        {
            prevDieButton.gameObject.SetActive(true);
        }
        //to here: trash. please do this better without ifs. or maybe not?
    }

}

[System.Serializable]
public class ForgeDieData
{
    public DieData dieData;
    public int currentFaceindex;

    public ForgeDieData(DieData _dieData)
    {
        dieData = _dieData;
        currentFaceindex = 0;
    }

    public DieFaceValue GetCurrentFaceValue()
    {
        return dieData.facesValues[currentFaceindex];
    }
}

[System.Serializable]
public struct DisplayDicePair
{
    public ForgeDisplayDie buffDie;
    public ForgeDisplayDie resourceDie;
}