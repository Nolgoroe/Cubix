using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollersManager : MonoBehaviour
{
    public static RollersManager Instance;

    [SerializeField] private List<DieRoller> rollers;

    private void Awake()
    {
        Instance = this;
    }

    public void Roll()
    {
        foreach (var roller in rollers)
        {
            roller.Roll();
        }
    }
}
