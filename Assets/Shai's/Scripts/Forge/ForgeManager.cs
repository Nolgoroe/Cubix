using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeManager : MonoBehaviour
{

    //tmp text for representation
    [SerializeField] private TMP_Text cubeNameTxt;
    [SerializeField] private TMP_Text faceNumTxt;
    [SerializeField] private TMP_Text resourceTypeTxt;
    [SerializeField] private TMP_Text buffTypeTxt;
    [SerializeField] private TMP_Text resourceValueTxt;
    [SerializeField] private TMP_Text buffValueTxt;

    [SerializeField] private List<ForgeDieData> dice;
    private int currentDieIndex;



    public void Init(List<Die> _dice)
    {
        AssignDice(_dice);

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
        UpdateCurrentDieView();
    }

    public void ChangeDieFaceIndexByStep(int step)
    {
        dice[currentDieIndex].currentFaceindex += step;
    }

    private void UpdateCurrentDieView()
    {
        ForgeDieData currentforgeDie = dice[currentDieIndex];

        cubeNameTxt.text = "Die: " + currentforgeDie.die.name;
        faceNumTxt.text = currentforgeDie.currentFaceindex.ToString();
        resourceTypeTxt.text = currentforgeDie.GetCurrentFace().GetFaceValue().Resource.Type.ToString();
        buffTypeTxt.text = currentforgeDie.GetCurrentFace().GetFaceValue().Buff.Type.ToString();
        resourceValueTxt.text = currentforgeDie.GetCurrentFace().GetFaceValue().Resource.Value.ToString();
        buffValueTxt.text = currentforgeDie.GetCurrentFace().GetFaceValue().Buff.Value.ToString();



    }

    public void ChangeCurrentFacePair(ResourceData resource, BuffData buff)
    {
        dice[currentDieIndex].GetCurrentFace().SetResource(resource);
        dice[currentDieIndex].GetCurrentFace().SetBuff(buff);
    }
    
    public void ChangeCurrentFaceResource(ResourceData resource)
    {
        dice[currentDieIndex].GetCurrentFace().SetResource(resource);

    }

    public void ChangeCurrentFaceBuff(BuffData buff)
    {
        dice[currentDieIndex].GetCurrentFace().SetBuff(buff);

    }

    public void UpgradeCurrentDieFace()
    {
        
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