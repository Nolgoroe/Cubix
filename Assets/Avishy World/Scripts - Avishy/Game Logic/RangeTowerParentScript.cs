using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTowerParentScript : TowerBaseParent
{
    [Header ("Live data")]
    [SerializeField] private Transform currentTarget;

    [Header("Preset Data")] 
    [SerializeField] private float range = 15;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] protected float fireRate = 1;
    [SerializeField] protected float fireCountDown = 0;
    [SerializeField] protected float bulltDMG = 1;
    [SerializeField] private Transform partToRotate;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;


    private float originalRange;
    private float originalRotationSpeed;
    private float originalFireRate;
    private float originalBulletDMG;

    protected override void Start()
    {
        originalRange = range;
        originalRotationSpeed = rotationSpeed;
        originalFireRate = fireRate;
        originalBulletDMG = bulltDMG;

        base.Start();

        SetRangeIndicator();
    }

    private void SetRangeIndicator()
    {
        //radius is half of the diameter of a circle
        if (rangeIndicator)
        {
            rangeIndicator.localScale = new Vector3(range * 2 / originalScale.x, range * 2 / originalScale.y, range * 2 / originalScale.z);
            rangeIndicator.gameObject.SetActive(false);
        }
    }
    protected virtual void Update()
    {
        if (GameManager.gamePaused) return;

        UpdateTarget();
        if (currentTarget == null) return;

        //locking on target
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed * GameManager.gameSpeed).eulerAngles;

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
            bullet.InitBullet(currentTarget, bulltDMG);
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
    {        

        currentCellOnPos = positionOfCell;

        towerDie = connectedDie;

        towerDie.transform.SetParent(resultDiceHolder);
        //SpawnBuffCubeOnCreation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public override void RecieveBuffAfterRoll(Die die)
    {
        DieFaceValue dieFaceValue = towerDie.GetTopValue();

        switch (dieFaceValue.Buff.Type)
        {
            case BuffType.None:
                break;
            case BuffType.Dmg:
                bulltDMG += originalBulletDMG * (dieFaceValue.Buff.Value / 100);
                break;
            case BuffType.Range:
                range += originalRange * (dieFaceValue.Buff.Value / 100);

                SetRangeIndicator();
                break;
            case BuffType.HP:
                break;
            case BuffType.time:
                break;
            default:
                break;
        }

        AddNewTowerBuff(dieFaceValue, die);
    }

    public override void OnHoverOverOccupyingCell(bool isHover)
    {
        if (rangeIndicator)
            rangeIndicator.gameObject.SetActive(isHover ? true : false);
    }
}
