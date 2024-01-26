using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTowerParentScript : TowerBaseParent
{
    [Header ("Live data")]
    [SerializeField] private Transform currentTarget;

    [Header("Local Combat")] 
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] protected float bulltDMG = 1;

    [Header("Combat Timers")]
    [SerializeField] protected float fireRate = 1;
    [SerializeField] protected float fireCountDown = 0;

    [SerializeField] protected float specialFireRate = 1;
    [SerializeField] protected float specialFireRateCooldown = 0;
    [SerializeField] protected bool isSpecialBullet = false;


    [Header("Preset Refs")]
    [SerializeField] private Transform partToRotate;
    [SerializeField] private Transform firePoint;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] private LayerMask enemyLayerMask;


    private float originalRange;
    private float originalRotationSpeed;
    private float originalFireRate;
    private float originalBulletDMG;

    protected override void Start()
    {
        fireCountDown = fireRate;
        specialFireRateCooldown = specialFireRate;
        originalRange = range;
        originalRotationSpeed = rotationSpeed;
        originalFireRate = fireRate;
        originalBulletDMG = bulltDMG;

        base.Start();

        SetRangeIndicator();
    }

    protected virtual void Update()
    {
        if (GameManager.gamePaused) return;

        UpdateTarget();
        if (currentTarget == null) return;

        //locking on target
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, (Time.deltaTime * rotationSpeed * GameManager.gameSpeed)).eulerAngles;

        partToRotate.rotation = Quaternion.Euler(rotation);

        if(specialAttackUnlocked)
        {
            if (specialFireRateCooldown <= 0)
            {
                isSpecialBullet = true;
                specialFireRateCooldown = specialFireRate;
            }

            specialFireRateCooldown -= Time.deltaTime * GameManager.gameSpeed;
        }

        if (fireCountDown <= 0)
        {
            Shoot();
            fireCountDown = fireRate;

            if(isSpecialBullet)
            {
                isSpecialBullet = false;
            }

            return;
        }

        fireCountDown -= Time.deltaTime * GameManager.gameSpeed;
    }

    private void Shoot()
    {
        GameObject go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        TowerBullet bullet;
        go.TryGetComponent<TowerBullet>(out bullet);
        
        if(bullet)
        {
            bullet.InitBullet(currentTarget, bulltDMG, isSpecialBullet);
        }
    }

    private void UpdateTarget()
    {
        if (fireCountDown <= fireCountDown / 5)
        {
            return;
        }

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

        specialAttackUnlocked = towerDie.ReturnSpecialAttackUnlcoked();
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
    
    public float ReturnRangeTower()
    {
        return range;
    }

    public override List<string> DisplayTowerStats()
    {
        List<string> stringList = new List<string>()
        { "Range: " + range.ToString(),
          "Required Color: " + requiredCellColorType.ToString(),
          "Special Unlcoked: " + (specialAttackUnlocked ? "True" : "False"),
          "Rotation Speed: " + rotationSpeed.ToString(),
          "Bullet Damage: " + bulltDMG.ToString(),
          "Fire Rate: " + fireRate.ToString(),
          "Special Attack Rate: " + specialFireRate.ToString()          
        };

        return stringList;
    }
}
