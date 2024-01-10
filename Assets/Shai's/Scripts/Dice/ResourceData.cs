using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType{Coins, Scraps }

[System.Serializable]
public class ResourceData 
{
    public ResourceType Type;
    public int Value;
    public Sprite Icon;
}
