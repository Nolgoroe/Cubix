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
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger("ToggleWeapon");
            isUsingWeapon = !isUsingWeapon;
        }
    }
}
