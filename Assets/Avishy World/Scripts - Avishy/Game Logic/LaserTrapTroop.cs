using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrapTroop : TowerTroop
{
    [SerializeField] float lifetime = 2;
    [SerializeField] bool activated = false;

    void Start()
    {
        health = Mathf.Infinity;

    }

    protected override void Update()
    {
        base.Update();

        if(currentTarget)
        {
            activated = true;
        }

        if(activated)
        {
            lifetime -= Time.deltaTime * GameManager.gameSpeed;

            if (lifetime < 0)
            {
                connectedTower.LoseTroop();
                Destroy(gameObject);
                return;
            }
        }
    }
}