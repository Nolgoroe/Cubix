using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DieFace : MonoBehaviour
{
    [SerializeField] private SpriteRenderer faceIcon;
    [SerializeField] private TMP_Text valueText;
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

    public void SetResource(ResourceType type, int value, Sprite icon)
    {
        resource.Type = type;
        resource.Value = value;
        resource.Icon = icon;
    }

    public void SetBuff(BuffData _buff)
    {
        buff = _buff;
    }

    public void SetBuff(BuffType type, int value, Sprite icon)
    {
        buff.Type = type;
        buff.Value = value;
        buff.Icon = icon;
    }

    public void DisplayBuff()
    {
        faceIcon.sprite = buff.Icon;
        valueText.text = "+" + buff.Value;
    }

    public void DisplayResource()
    {
        faceIcon.sprite = resource.Icon;
        valueText.text = "+" + resource.Value;
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

