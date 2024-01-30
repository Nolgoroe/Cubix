using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public static PlayerWeaponManager Instance;

    public static bool isUsingWeapon = false;

    [SerializeField] Animator anim;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();

        isUsingWeapon = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && !GameManager.playerTurn)
        {
            ToggleWeapon(!isUsingWeapon);
        }
    }

    public void ToggleWeapon(bool isOn)
    {
        isUsingWeapon = isOn;
        anim.SetTrigger("ToggleWeapon");
    }
}
