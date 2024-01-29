using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeResource : MonoBehaviour
{
    [SerializeField] private ResourceData resource;
    [SerializeField] private TMP_Text buttonTxt;
    [SerializeField] private ForgeManager forge;

    public void Init(ForgeManager _forge, ResourceData _resource)
    {
        forge = _forge;
        resource = _resource;
        if (buttonTxt)
        {
            buttonTxt.text = resource.Value.ToString() + " " + resource.Type.ToString();
        }
    }

    public void SetForgeResource()
    {
        forge.SetForgeCurrentEditResource(resource);
        forge.ChangeCurrentFaceResource();
    }

    private void OnValidate()
    {
        if (buttonTxt)
        {
            buttonTxt.text = resource.Value.ToString() + " " + resource.Type.ToString();
        }
    }
}
