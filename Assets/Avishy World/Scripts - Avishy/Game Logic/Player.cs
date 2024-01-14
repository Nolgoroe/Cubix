using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private List<Die> allDieInPlay;
    [SerializeField] private int coins;
    [SerializeField] private int scraps;

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
}
