using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForgeBuff : MonoBehaviour
{
    [SerializeField] private BuffData buff;
    [SerializeField] private TMP_Text buttonTxt;
    [SerializeField] private ForgeManager forge;



    public void Init(ForgeManager _forge, BuffData _buff)
    {
        forge = _forge;
        buff = _buff;
        buttonTxt.text = buff.Value.ToString() + " " + buff.Type.ToString();
    }

    public void SetForgeBuff()
    {
        forge.SetForgeCurrentEditBuff(buff);
        forge.ChangeCurrentFaceBuff();
    }

    private void OnValidate()
    {
        buttonTxt.text = buff.Value.ToString() + " " + buff.Type.ToString();
    }
}
