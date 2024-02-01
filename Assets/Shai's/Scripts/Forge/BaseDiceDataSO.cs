using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseDiceData", menuName = "ScriptableObjects/BaseDice")]
public class BaseDiceDataSO : ScriptableObject
{
    [SerializeField] private List<DieData> baseDice;

    public DieFaceValue GetBaseFaceValueOfDie(CellTypeColor color, DieType type, int faceNum)
    {
        foreach (var die in baseDice)
        {
            if (die.dieType == type && die.colorType == color)
            {
                return die.facesValues[faceNum];
            }
        }
        Debug.LogError("Forge trying to get info about a base die that does'nt exsist");
        return null;
    }
}
