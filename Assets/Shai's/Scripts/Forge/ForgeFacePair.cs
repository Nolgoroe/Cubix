using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeFacePair : MonoBehaviour
{

    public int price;

    [SerializeField] private ForgeManager forge;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private DieFaceValue faceValue;

    private void Start()
    {
        priceText.text = "Price: " + price;
    }

    public void Init(ForgeManager _forge, DieFaceValue _faceValue)
    {
        forge = _forge;
        faceValue = _faceValue;

        buttonText.text = faceValue.Buff.Value + " " + faceValue.Buff.Type + "/" +
                     faceValue.Resource.Value + " " + faceValue.Resource.Type;
    }

    public void ChangeDieFace()
    {
        if (Player.Instance)
        {
            //if price is scrap
            //Player.Instance.AddRemoveScrap(-price);

            //if else
            Player.Instance.RemoveResources(ResourceType.Iron, price);
        }

        forge.SetForgeCurrentEditFacePair(faceValue);
        forge.ChangeCurrentFacePair();
    }

    private void OnValidate()
    {
        buttonText.text = faceValue.Buff.Value + " " + faceValue.Buff.Type + "/" +
                     faceValue.Resource.Value + " " + faceValue.Resource.Type;
    }
     
    
    
}
