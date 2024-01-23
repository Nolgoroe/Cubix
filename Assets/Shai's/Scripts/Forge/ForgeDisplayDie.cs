using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeDisplayDie : MonoBehaviour
{
    [SerializeField] private TMP_Text valTxt;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private List<MeshRenderer> facesMesh;

    public void UpdateDisplay(Material dieMat, string valString, Sprite iconSprite)
    {
        foreach (var face in facesMesh)
        {
            face.material = dieMat;
        }

        valTxt.text = valString;
        icon.sprite = iconSprite;
    }
}
