using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrapTroop : TowerTroop
{
    [SerializeField] float lifetime = 2;
    [SerializeField] bool activated = false;

    override protected void Start()
    {
        health = Mathf.Infinity;
        base.Start();
    }

    protected override void Update()
    {
        if (GameManager.gameSpeed == 0) return;

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
                connectedTower.LoseTroop(this);
                Destroy(gameObject);
                return;
            }
        }
    }
}
