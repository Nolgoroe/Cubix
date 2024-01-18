using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTroop : MonoBehaviour
{
    //this might turn to a parent script for any and all "tower troops" in the future for different towers that require differnt troops

    [Header("Live data")]
    [SerializeField] protected Transform currentTarget;
    [SerializeField] protected MeleeTowerParentScript connectedTower;

    [Header("Preset Data")] 
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float currentAttackCooldown = 0;
    [SerializeField] protected float rotationSpeed = 10;
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] private LayerMask enemyLayerMask;

    [SerializeField] protected float health = 3;

    [SerializeField] private float range = 15;
    [SerializeField] private float damage = 1;
    bool isDead;

    virtual protected void Start()
    {
        SetRangeIndicator();
    }

    private void SetRangeIndicator()
    {
        if (rangeIndicator)
        {
            rangeIndicator.localScale = new Vector3(range * 2 / transform.lossyScale.x, range * 2 / transform.lossyScale.y, range * 2 / transform.lossyScale.z);
            rangeIndicator.gameObject.SetActive(false);
        }
    }
    protected virtual void Update()
    {
        if (GameManager.gamePaused) return;

        UpdateTarget();

        if(currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * (rotationSpeed * GameManager.gameSpeed));

        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = (1 / attackRate) / GameManager.gameSpeed;
        }

    }

    private void Attack()
    {
        EnemyParent enemyHit;
        currentTarget.TryGetComponent<EnemyParent>(out enemyHit);

        if (enemyHit)
        {
            enemyHit.RecieveDMG(damage);
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

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        if (nearestEnemy && shortestDistance <= range)
        {
            currentTarget = nearestEnemy.transform;
        }
        else
        {
            currentTarget = null;
        }
    }




    public void OnHoverOverParentTower(bool isHover)
    {
        if (rangeIndicator)
        {
            rangeIndicator.gameObject.SetActive(isHover ? true : false);
        }
    }

    public void InitTroopData(MeleeTowerParentScript tower, float _HP, float _range, float _dmg)
    {
        connectedTower = tower;
        health = _HP;
        range = _range;
        damage = _dmg;

        SetRangeIndicator();
    }

    public void RecieveDMG(int damage)
    {
        if (isDead) return;
        if (health <= 0)
        {
            isDead = true;
            Debug.Log("Recieve dmg");
            Destroy(gameObject);

            connectedTower.LoseTroop(this);

            return;
        }

        health -= damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
