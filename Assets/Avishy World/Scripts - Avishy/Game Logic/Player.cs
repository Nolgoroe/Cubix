using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private List<Die> allDieInPlay;
    [SerializeField] private int coins;
    [SerializeField] private int scraps;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allDieInPlay = new List<Die>();
        allDieInPlay.AddRange(FindObjectsOfType<Die>());

        foreach (Die die in allDieInPlay)
        {
            die.OnRollEndEvent.AddListener(AddResourcesAfterRoll);
        }
    }


    private void AddResourcesAfterRoll(Die die)
    {
        if (die.ReturnInWorld()) return;


        DieFaceValue dieFaceVakue = die.GetTopValue();

        switch (dieFaceVakue.Resource.Type)
        {
            case ResourceType.Coins:
                coins+= dieFaceVakue.Resource.Value;
                break;
            case ResourceType.Scraps:
                scraps += dieFaceVakue.Resource.Value;
                break;
            default:
                break;
        }
    }

    public void RecieveRandomResource()
    {
        ResourceType[]  myEnums = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        int randomResource = UnityEngine.Random.Range(0, myEnums.Length);

        switch (myEnums[randomResource])
        {
            case ResourceType.Coins:
                coins += 10;
                break;
            case ResourceType.Scraps:
                scraps += 10;
                break;
            default:
                break;
        }
    }

}
