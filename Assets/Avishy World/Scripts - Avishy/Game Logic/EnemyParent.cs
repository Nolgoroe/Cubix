using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParent : MonoBehaviour
{
    [Header("Enemy Stats")]// this is all temp - will be an SO later... maybe
    [SerializeField] private float Speed;
    [SerializeField] private Transform target;
    [SerializeField] private int enemyDamage;

    [Header("Path Data")]
    [SerializeField] private int waypointIndex;
    [SerializeField] private float waypointDetectionRadius;
    [SerializeField] private List<GridCell> waypointsList;


    private void FixedUpdate()
    {
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * Speed * Time.fixedDeltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);


        if (Vector3.Distance(transform.position, target.position) < waypointDetectionRadius)
        {
            GetNextWaypoint();
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








    public void InitEnemy(List<GridCell> waypoints)
    {
        waypointsList = waypoints;
        target = waypoints[1].transform; // the first waypoint is the spawner position, we don't need it here
    }


    public int ReturnEnemyDMG()
    {
        return enemyDamage;
    }
}
