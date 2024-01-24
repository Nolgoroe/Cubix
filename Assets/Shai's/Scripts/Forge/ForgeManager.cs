using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeManager : MonoBehaviour
{
    [Header("Text Display")]
    [SerializeField] private TMP_Text cubeNameTxt;
    [SerializeField] private TMP_Text faceNumTxt;
    [SerializeField] private TMP_Text resourceTypeTxt;
    [SerializeField] private TMP_Text buffTypeTxt;
    [SerializeField] private TMP_Text resourceValueTxt;
    [SerializeField] private TMP_Text buffValueTxt;

    [Header("DieDisplay")]
    [SerializeField] private DisplayDicePair d6Die;
    [SerializeField] private DisplayDicePair d8Die;

    [Header("Settings")]
    [SerializeField] private List<ForgeDieData> dice;
    [SerializeField] private Die blankD6Prefab;
    [SerializeField] private Die blankD8Prefab;

    private DisplayDicePair currentDisplayDice;

    private int currentDieIndex;
    private ResourceData _currentEditResource;
    private BuffData _currentEditBuff;

    [Header("Temp Settings")]//for standalone testing
    [SerializeField] private List<Die> realDice;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Init(TempDieDataExtractor());
        }
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
        UpdateCurrentDieView();
    }

    public void ChangeDieFaceIndexByStep(int step)
    {
        ForgeDieData currentforgeDie = dice[currentDieIndex];

        currentforgeDie.currentFaceindex += step;
        currentforgeDie.currentFaceindex = Mathf.Clamp(currentforgeDie.currentFaceindex, 0, currentforgeDie.dieData.facesValue.Count - 1);
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
        switch (currentDie.dieData.DieType)
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
        return dieData.facesValue[currentFaceindex];
    }
}

[System.Serializable]
public struct DisplayDicePair
{
    public ForgeDisplayDie buffDie;
    public ForgeDisplayDie resourceDie;
}