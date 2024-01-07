using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieFace : MonoBehaviour
{
    public SpriteRenderer FaceDisplay;
    public ResourceData Resource;
    public BuffData Buff;

    public DieFaceValue GetFaceValue()
    {
        return new DieFaceValue(Resource, Buff);
    }
}

public struct DieFaceValue
{
    private ResourceData resource;
    private BuffData buff;

    public ResourceData Resource { get { return resource; } }
    public BuffData Buff { get { return buff; } }

    public DieFaceValue(ResourceData _resource, BuffData _buff)
    {
        resource = _resource;
        buff = _buff;
    }
}

