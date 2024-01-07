using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { Speed, Damage, Fire}

public class BuffData : MonoBehaviour
{
    public BuffType Type;
    public int Value;
    public Sprite Icon;
}
