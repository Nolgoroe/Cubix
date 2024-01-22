using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    //this might turn to a parent script for any and all "tower bullets" in the future for different towers that require differnt bullets

    [SerializeField] Transform currentTarget;
    [SerializeField] GameObject colldiedObject;
    [SerializeField] GameObject chainLightningEffect; //temp here

    [SerializeField] float speed = 10;
    [SerializeField] private float damage = 1;
    
    
    void Update()
    {
        //if (GameManager.gameSpeed == 0) return;

        //if (currentTarget == null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}


        //Vector3 direction = currentTarget.position - transform.position;
        //float distanceThisFrame = (speed * GameManager.gameSpeed) * Time.deltaTime; // used to calculate my position in this frame to prevent overshooting

        ////direction.magnitude is the current distance to our taget. 
        ////if that current distance is less than the amount we want to move, that means we've hit the target and that next frame we'll pass it.
        //if (direction.magnitude <= distanceThisFrame)
        //{
        //    HitTarget();
        //    return;
        //}

        ////without normalising the movement here, the further away we'll be of the object, the faster we will move.
        //// normalized return this vector with a magnitude of 1, meaning it's length is 1 but direction is the same.
        //transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        EnemyParent enemyHit;
        currentTarget.TryGetComponent<EnemyParent>(out enemyHit);
        
        if(enemyHit)
        {
            enemyHit.RecieveDMG(damage);
        }

        //this is temp here - will move to new bullet script. a bullet that will be shot on a different cooldown.
        Instantiate(chainLightningEffect, currentTarget.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }




    public void OnCollidedWithObject(GameObject collidedWith)
    {
        if(collidedWith.transform == currentTarget)
        {
            HitTarget();
        }
    }


    public void InitBullet(Transform target, float bulletDMG)
    {
        currentTarget = target;
        damage = bulletDMG;
    }
}
