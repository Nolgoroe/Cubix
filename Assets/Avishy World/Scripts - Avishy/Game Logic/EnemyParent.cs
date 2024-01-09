using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParent : MonoBehaviour
{
    [Header("Enemy Stats")]// this is all temp - will be an SO later... maybe
    [SerializeField] private float Speed;
    [SerializeField] private float range = 0.5f;
    [SerializeField] private float enemyHealth = 3;
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float currentAttackCooldown = 0;
    [SerializeField] private int enemyDamage;
    [SerializeField] private bool ignoresTroops;
    [SerializeField] private LayerMask playerTroopsLayer;
    [SerializeField] private Transform target;

    [Header("Live Data")]
    [SerializeField] private Transform currentTarget;

    [Header("Path Data")]
    [SerializeField] private int waypointIndex;
    [SerializeField] private float waypointDetectionRadius;
    [SerializeField] private List<GridCell> waypointsList;

    private void Update()
    {
        UpdateTarget();

        if (currentAttackCooldown > 0)
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
    private void FixedUpdate()
    {
        if (!ignoresTroops && currentTarget) return;

        Vector3 direction = target.position - transform.position;
        transform.Translate((direction.normalized * Speed * GameManager.gameSpeed) * Time.fixedDeltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);


        if (Vector3.Distance(transform.position, target.position) < waypointDetectionRadius)
        {
            GetNextWaypoint();
        }
    }
    private void Attack()
    {
        TowerTroop troopHit;
        currentTarget.TryGetComponent<TowerTroop>(out troopHit);

        if (troopHit)
        {
            troopHit.RecieveDMG(enemyDamage);
        }
    }

    private void GetNextWaypoint()
    {
        if(waypointIndex >= waypointsList.Count - 1)
        {
            //Destroy(gameObject);
            return;
        }

        waypointIndex++;
        target = waypointsList[waypointIndex].transform;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("PlayerBase"))
        {
            PlayerHomeBaseCell playerBase;

            collision.transform.TryGetComponent<PlayerHomeBaseCell>(out playerBase);

            if(playerBase)
            {
                playerBase.RecieveDamage(this);
            }

            Destroy(gameObject);
        }
    }

    private void UpdateTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, playerTroopsLayer);

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






    public void RecieveDMG(float amount)
    {
        enemyHealth -= amount;

        if(enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void InitEnemy(List<GridCell> waypoints)
    {
        waypointsList = waypoints;
        target = waypoints[1].transform; // the first waypoint is the spawner position, we don't need it here
    }


    public int ReturnEnemyDMG()
    {
        return enemyDamage;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
