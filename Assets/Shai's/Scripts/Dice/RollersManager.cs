using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollersManager : MonoBehaviour
{
    [SerializeField] private List<DieRoller> rollers;

    public void Roll()
    {
        foreach (var roller in rollers)
        {
            roller.Roll();
        }
    }
}
