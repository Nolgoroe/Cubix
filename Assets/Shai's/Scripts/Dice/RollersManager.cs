using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollersManager : MonoBehaviour
{
    public static RollersManager Instance;

    [SerializeField] private List<DieRoller> resourceDice;
    [SerializeField] private List<DieRoller> worldDice;

    private void Awake()
    {
        Instance = this;
    }

    public void RollResources()
    {
        foreach (var roller in resourceDice)
        {
            roller.Roll();
        }
    }
    public void RollInWorld()
    {
        foreach (var roller in worldDice)
        {
            roller.Roll();
        }
    }

    //the next 4 functions are temp - find better way
    public void AddDiceToResources(DieRoller die) //temp
    {
        resourceDice.Add(die);
    }
    public void RemoveDiceToResources(DieRoller die) //temp
    {
        resourceDice.Remove(die);
    }

    public void AddDiceToWorld(DieRoller die) //temp
    {
        worldDice.Add(die);
    }
    public void RemoveDiceToWorld(DieRoller die) //temp
    {
        worldDice.Remove(die);
    }
}
