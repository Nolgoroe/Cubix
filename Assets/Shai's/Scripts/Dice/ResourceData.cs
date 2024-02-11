using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType{Iron, Nova, Energy, scrap, None}

[System.Serializable]
public class ResourceData 
{
    public ResourceType Type;
    public int Value;
    public Sprite Icon;
}
