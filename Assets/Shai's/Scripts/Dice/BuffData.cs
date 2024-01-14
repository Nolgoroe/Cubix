using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { Speed, Damage, Fire, AttackSpeed}

[System.Serializable]
public class BuffData 
{
    public BuffType Type;
    public int Value;
    public Sprite Icon;
}
