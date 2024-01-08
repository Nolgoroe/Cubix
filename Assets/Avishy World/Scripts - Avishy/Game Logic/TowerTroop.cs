using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTroop : MonoBehaviour
{
    //this might turn to a parent script for any and all "tower troops" in the future for different towers that require differnt troops

    [Header("Live data")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private MeleeTowerParentScript connectedTower;

    [Header("Preset Data")] // this is temp - might be scriptable object
    [SerializeField] private float range = 15;
    [SerializeField] private float damage = 1;
    [SerializeField] private float health = 3;
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float currentAttackCooldown = 0;
    [SerializeField] private LayerMask enemyLayerMask;

    private void Update()
    {
        UpdateTarget();

        if(currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (currentTarget == null) return;

        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = (1 / attackRate) / GameManager.gameSpeed;
        }

    }

    //this will need to change to a "type" of towers that can spawn troops, think of a better way,
    //what will happen if we have another tower except the cyber tower that spawns troops, this can't stay only "CyberTower"
    public void InitTroopData(MeleeTowerParentScript tower)  
    {
        connectedTower = tower;
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

    public void RecieveDMG(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            connectedTower.LoseTroop();
            Destroy(gameObject);
            return;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
