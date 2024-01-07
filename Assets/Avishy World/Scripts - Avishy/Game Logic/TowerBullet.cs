using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    [SerializeField] Transform currentTarget;

    [SerializeField] float speed = 10;
    [SerializeField] float damage = 1;
    
    
    void Update()
    {
        if(currentTarget == null)
        {
            Destroy(gameObject);
            return;
        }


        Vector3 direction = currentTarget.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime; // used to calculate my position in this frame to prevent overshooting

        //direction.magnitude is the current distance to our taget. 
        //if that current distance is less than the amount we want to move, that means we've hit the target and that next frame we'll pass it.
        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        //without normalising the movement here, the further away we'll be of the object, the faster we will move.
        // normalized return this vector with a magnitude of 1, meaning it's length is 1 but direction is the same.
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        EnemyParent enemyHit;
        currentTarget.TryGetComponent<EnemyParent>(out enemyHit);
        
        if(enemyHit)
        {
            enemyHit.RecieveDMG(damage);
        }

        Destroy(gameObject);
    }

    public void InitBullet(Transform target)
    {
        currentTarget = target;
    }
}
