using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSlot : MonoBehaviour
{
    [SerializeField] private Transform lockTransform;
    //[SerializeField] private Transform resultDiceTransform;


    public Transform ReturnLockTransform()
    {
        return lockTransform;
    }
    //public Transform ReturnResultDiceTransform()
    //{
    //    return resultDiceTransform;
    //}
}
