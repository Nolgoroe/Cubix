using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    [SerializeField] private List<ForgeDieData> dies;
    private int currentDieIndex;


    public void SetDieIndex(int newIndex)
    {
        currentDieIndex = newIndex;
    }

    public void ChangeDieFaceIndexByStep(int step)
    {
        dies[currentDieIndex].currentFaceindex += step;
    }

    private void UpdateCurrentDieView()
    {
        
    }

    public void ChangeCurrentFacePair(ResourceData resource, BuffData buff)
    {
        dies[currentDieIndex].GetCurrentFace().SetResource(resource);
        dies[currentDieIndex].GetCurrentFace().SetBuff(buff);
    }
    
    public void ChangeCurrentFaceResource(ResourceData resource)
    {
        dies[currentDieIndex].GetCurrentFace().SetResource(resource);

    }

    public void ChangeCurrentFaceBuff(BuffData buff)
    {
        dies[currentDieIndex].GetCurrentFace().SetBuff(buff);

    }

    public void UpgradeCurrentDieFace()
    {
        //idea: we should add each tower damage\resource a modidier that will increase/descrease its
        //damage\amount by a percantage and this method will only increase it 
    }

    public void UpgradeCurrentDie()
    {

    }


    
}

public class ForgeDieData
{
    public Die die;
    public int currentFaceindex;

    public DieFace GetCurrentFace()
    {
        return die.GetAllFaces()[currentFaceindex];
    }
}