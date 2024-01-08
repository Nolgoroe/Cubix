using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieFace : MonoBehaviour
{
    public int val;
    [SerializeField] private SpriteRenderer FaceDisplay;
    [SerializeField] private ResourceData resource;
    [SerializeField] private BuffData buff;

    public DieFaceValue GetFaceValue()
    {
        return new DieFaceValue(resource, buff);
    }

    public void SetResource(ResourceData _resource)
    {
        resource = _resource;
    }

    public void SetResource(ResourceType type, int value)
    {
        resource.Type = type;
        resource.Value = value;
    }

    public void SetBuff(BuffData _buff)
    {
        buff = _buff;
    }

    public void SetBuff(BuffType type, int value)
    {
        buff.Type = type;
        buff.Value = value;
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

