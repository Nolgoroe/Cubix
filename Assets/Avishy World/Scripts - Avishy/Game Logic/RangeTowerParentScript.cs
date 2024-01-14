using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTowerParentScript : TowerBaseParent
{
    [Header ("Live data")]
    [SerializeField] private Transform currentTarget;

    [Header("Preset Data")] // this is temp - might be scriptable object
    [SerializeField] private float range = 15;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] protected float fireRate = 1;
    [SerializeField] protected float fireCountDown = 0;
    [SerializeField] private Transform partToRotate;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    protected virtual void Update()
    {
        if (GameManager.gameSpeed == 0) return;
        UpdateTarget();
        if (currentTarget == null) return;

        //locking on target
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * (rotationSpeed * GameManager.gameSpeed)).eulerAngles;

        partToRotate.rotation = Quaternion.Euler(rotation);

        if(fireCountDown <= 0)
        {
            Shoot();
            fireCountDown = (1 / fireRate) / GameManager.gameSpeed;
        }

        fireCountDown -= Time.deltaTime;
    }

    private void Shoot()
    {
        Debug.Log("Shoot");

        GameObject go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        TowerBullet bullet;
        go.TryGetComponent<TowerBullet>(out bullet);
        
        if(bullet)
        {
            bullet.InitBullet(currentTarget);
        }
    }

    private void UpdateTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, enemyLayerMask);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider enemy in hitColliders)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        if(nearestEnemy && shortestDistance <= range)
        {
            currentTarget = nearestEnemy.transform;
        }
        else
        {
            currentTarget = null;
        }
    }







    public override void InitTowerData(Vector2Int positionOfCell, Die connectedDie)
    {        //new avishy

        currentCellOnPos = positionOfCell;

        towerDie = connectedDie;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public override void RecieveBuffAfterRoll(Die die)
    {
        DieFaceValue dieFaceVakue = towerDie.GetTopValue();

        switch (dieFaceVakue.Buff.Type)
        {
            case BuffType.Speed:
                break;
            case BuffType.Damage:
                break;
            case BuffType.Fire:
                break;
            case BuffType.AttackSpeed:
                //add attack speed to relavent tower
                fireRate += 0.1f;
                break;
            default:
                break;
        }


    }
}
