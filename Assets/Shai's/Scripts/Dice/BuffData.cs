using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { None, Dmg, HP, time, Range}

[System.Serializable]
public class BuffData 
{
    public BuffType Type;
    public float Value;
    public Sprite Icon;
}
