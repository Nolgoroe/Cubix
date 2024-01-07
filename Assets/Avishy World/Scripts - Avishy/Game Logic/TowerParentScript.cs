using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerParentScript : MonoBehaviour
{
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float range = 15;
    [SerializeField] private Transform partToRotate;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private LayerMask enemyLayerMask;


    private void Update()
    {
        UpdateTarget();
        if (currentTarget == null) return;

        //locking on target
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;

        partToRotate.rotation = Quaternion.Euler(0, rotation.y, 0);
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
