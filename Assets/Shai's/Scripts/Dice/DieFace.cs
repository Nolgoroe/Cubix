using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DieFace : MonoBehaviour
{
    [SerializeField] private Transform displayObject;
    [SerializeField] private SpriteRenderer faceIcon;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private ResourceData resource;
    [SerializeField] private BuffData buff;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private Vector3 orientationEndRoll;
    [SerializeField] private Vector3 orientationEndRollInWorld;

    public Transform DisplayObject { get { return displayObject; } }

    private void OnValidate()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    public DieFaceValue GetFaceValue()
    {
        return new DieFaceValue(resource, buff);
    }

    public void SetResource(ResourceData _resource)
    {
        resource = _resource;
        resource.Type = _resource.Type;
        resource.Value = _resource.Value;
        resource.Icon = _resource.Icon;
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
        buff.Type = _buff.Type;
        buff.Value = _buff.Value;
        buff.Icon = _buff.Icon;

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
        if(buff.Type == BuffType.None)
        {
            valueText.gameObject.SetActive(false);
        }
        else
        {
            valueText.gameObject.SetActive(true);

            valueText.text = "+" + buff.Value;
        }
    }

    public void DisplayResource()
    {
        faceIcon.sprite = resource.Icon;
        valueText.text = "+" + resource.Value;
    }

    public void ChangeFaceMat(Material mat)
    {
        renderer.material = mat;
    }

    public Vector3 ReturnOrientationOnEndRoll()
    {
        return orientationEndRoll;
    }
    public Vector3 ReturnOrientationOnEndRollWorld()
    {
        return orientationEndRollInWorld;
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

