using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeFacePair : MonoBehaviour
{
    [SerializeField] private ForgeManager forge;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private DieFaceValue faceValue;



    public void Init(ForgeManager _forge, DieFaceValue _faceValue)
    {
        forge = _forge;
        faceValue = _faceValue;

        buttonText.text = faceValue.Buff.Value + " " + faceValue.Buff.Type + "/" +
                     faceValue.Resource.Value + " " + faceValue.Resource.Type;
    }

    public void ChangeDieFace()
    {
        forge.SetForgeCurrentEditFacePair(faceValue);
        forge.ChangeCurrentFacePair();
    }

    private void OnValidate()
    {
        buttonText.text = faceValue.Buff.Value + " " + faceValue.Buff.Type + "/" +
                     faceValue.Resource.Value + " " + faceValue.Resource.Type;
    }
}
