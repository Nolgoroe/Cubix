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

    private DisplayDicePair currentDisplayDice;

    [SerializeField] private List<ForgeDieData> dice;
    private int currentDieIndex;
    private ResourceData _currentEditResource;
    private BuffData _currentEditBuff;

    public void Init(List<Die> _dice)
    {
        AssignDice(_dice);
        UpdateCurrentDieView();
    }

    public void AssignDice(List<Die> _dice)
    {
        dice.Clear();

        //assign new
        foreach (var die in _dice)
        {
            dice.Add(new ForgeDieData(die));
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
        currentforgeDie.currentFaceindex = Mathf.Clamp(currentforgeDie.currentFaceindex, 0, currentforgeDie.die.GetAllFaces().Length - 1);
        UpdateCurrentDieView();
    }

    private void UpdateCurrentDieView()
    {
        ForgeDieData currentDie = dice[currentDieIndex];
        //temp text diplay, change it to actual die display later
        cubeNameTxt.text = "Die: " + currentDie.die.name;
        faceNumTxt.text = (currentDie.currentFaceindex + 1).ToString();
        resourceTypeTxt.text = currentDie.GetCurrentFace().GetFaceValue().Resource.Type.ToString();
        buffTypeTxt.text = currentDie.GetCurrentFace().GetFaceValue().Buff.Type.ToString();
        resourceValueTxt.text = currentDie.GetCurrentFace().GetFaceValue().Resource.Value.ToString();
        buffValueTxt.text = currentDie.GetCurrentFace().GetFaceValue().Buff.Value.ToString();

        //update display buff die
        //currentDisplayDice.buffDie.UpdateDisplay(
        //    currentDie.GetCurrentFace().
        //    currentDie.GetCurrentFace().GetFaceValue().Buff.Type.ToString(),
        //    currentDie.GetCurrentFace().GetFaceValue().Buff.Icon);
    }

    public void ChangeCurrentFacePair()
    {
        dice[currentDieIndex].GetCurrentFace().SetResource(_currentEditResource);
        dice[currentDieIndex].GetCurrentFace().SetBuff(_currentEditBuff);

        UpdateCurrentDieView();
    }
    
    public void ChangeCurrentFaceResource()
    {
        dice[currentDieIndex].GetCurrentFace().SetResource(_currentEditResource);
        UpdateCurrentDieView();
        dice[currentDieIndex].die.DisplayResources();
    }

    public void ChangeCurrentFaceBuff()
    {
        dice[currentDieIndex].GetCurrentFace().SetBuff(_currentEditBuff);
        UpdateCurrentDieView();
        dice[currentDieIndex].die.DisplayBuffs();

    }

    public void UpgradeCurrentDieFace()
    {
        dice[currentDieIndex].GetCurrentFace().UpgradeFace(2, 2);
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
    public Die die;
    public int currentFaceindex;

    public ForgeDieData(Die _die)
    {
        die = _die;
        currentFaceindex = 0;
    }

    public DieFace GetCurrentFace()
    {
        return die.GetAllFaces()[currentFaceindex];
    }
}

[System.Serializable]
public struct DisplayDicePair
{
    public ForgeDisplayDie buffDie;
    public ForgeDisplayDie resourceDie;
}