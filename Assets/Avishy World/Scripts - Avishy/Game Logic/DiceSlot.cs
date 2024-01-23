using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSlot : MonoBehaviour
{
    [SerializeField] private Transform lockTransform;


    public Transform ReturnLockTransform()
    {
        return lockTransform;
    }
}
