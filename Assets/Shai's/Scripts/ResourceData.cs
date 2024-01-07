using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType{Coins, Scraps }

public class ResourceData : MonoBehaviour
{
    public ResourceType Type;
    public int Value;
    public Material FrameMaterial;
}
