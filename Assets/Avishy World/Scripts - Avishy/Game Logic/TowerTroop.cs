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
    [SerializeField] private float range = 15;
    [SerializeField] private float damage = 1;
    [SerializeField] protected float health = 3;
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float currentAttackCooldown = 0;
    [SerializeField] protected float rotationSpeed = 10;
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] private LayerMask enemyLayerMask;

    private void Start()
    {
        if (rangeIndicator)
            rangeIndicator.localScale = new Vector3(range * 2 / rangeIndicator.lossyScale.x, range * 2 / rangeIndicator.lossyScale.y, range * 2 / rangeIndicator.lossyScale.z);

    }
    protected virtual void Update()
    {
        if (GameManager.gameSpeed == 0) return;

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






    public void InitTroopData(MeleeTowerParentScript tower)
    {
        connectedTower = tower;
    }

    public void RecieveDMG(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            connectedTower.LoseTroop();
            Destroy(gameObject);
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
