using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes
{
    None,
    basicEnemy,
    flying,
    swimming
}
public class EnemyParent : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float range = 0.5f;
    [SerializeField] private float stopRange = 0.5f;
    [SerializeField] private float rotationSpeed = 2;
    [SerializeField] private float enemyHealth = 3;
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float currentAttackCooldown = 0;
    [SerializeField] private int enemyDamage;
    [SerializeField] private bool ignoresTroops;
    [SerializeField] private LayerMask playerTroopsLayer;
    [SerializeField] private Transform target;
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private float speedModifier = 1;
    [SerializeField] private float timeToEndSlow;
    [SerializeField] private float currentTimeSlowed;

    [Header("Economy Data")]
    [SerializeField] float percentageToGiveScrap;
    [SerializeField] int scrapAmount;

    [Header("Live Data")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float startingHeight;
    [SerializeField] private Renderer rend;
    [SerializeField] private Animator anim;

    [Header("Path Data")]
    [SerializeField] private Transform pathChecker;
    [SerializeField] private int waypointIndex;
    [SerializeField] private float waypointDetectionRadius;
    [SerializeField] private List<GridCell> waypointsList;

    [Header("Materials Data")]
    [SerializeField] protected Material defaultMat;
    [SerializeField] protected Material reachPlayerBaseMat;
    [SerializeField] protected Material spawnMat;
    [SerializeField] protected Material hitMat;
    [SerializeField] protected float timeToChangeToDefaultMatAtStart;
    [SerializeField] protected float timeToDieOnReachPlayerBase;
    [SerializeField] protected float timeToDisplayHit;
    [SerializeField] protected float timeToChangeToDefaultMatAfterHit;
    [SerializeField] protected string materialKeyGetHit;
    [SerializeField] protected string materialKeyReachBase;
    [SerializeField] protected Renderer[] renderersToFadeOnHitPlayer;

    [Header("Particles Data")]
    [SerializeField] protected GameObject onDeathParticle;
    [SerializeField] protected float timeToDie;

    private void Start()
    {
        SoundManager.Instance.PlaySoundOneShot(Sounds.EnemySpawn);

        startingHeight = transform.position.y;

        rend.material = spawnMat;

        ShadersControl.doNow = true;
        StartCoroutine(Helpers.SetMat(rend, defaultMat, timeToChangeToDefaultMatAtStart / GameManager.gameSpeed));
    }
    private void Update()
    {
        if (GameManager.gamePaused) return;

        UpdateTarget();

        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime * GameManager.gameSpeed;
        }

        if(currentTimeSlowed > 0)
        {
            currentTimeSlowed -= Time.deltaTime * GameManager.gameSpeed;

            if(currentTimeSlowed <= 0)
            {
                speedModifier = 1;
            }
        }

        if (currentTarget == null) return;

        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = attackRate;
        }
    }
    
    private void FixedUpdate()
    {
        anim.SetBool("Is Walking", false);

        if (GameManager.gamePaused) return;

        if (!ignoresTroops && currentTarget)
        {
            Vector3 direction = currentTarget.position - transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * (rotationSpeed * GameManager.gameSpeed));

            if(Vector3.Distance(transform.position, currentTarget.position) > stopRange)
            {
                anim.SetBool("Is Walking", true);

                transform.Translate((direction.normalized * speed * speedModifier * GameManager.gameSpeed) * Time.fixedDeltaTime, Space.World);
            }

            return;
        }
        else
        {
            anim.SetBool("Is Walking", true);

            Vector3 direction = target.position - transform.position;
            transform.Translate((direction.normalized * speed * speedModifier * GameManager.gameSpeed) * Time.fixedDeltaTime, Space.World);
            transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * (rotationSpeed * GameManager.gameSpeed)).eulerAngles;

            transform.rotation = Quaternion.Euler(0, rotation.y, 0);

            if (Vector3.Distance(pathChecker.transform.position, target.position) < waypointDetectionRadius)
            {
                GetNextWaypoint();
            }
        }
    }
    private void Attack()
    {
        TowerTroop troopHit;
        currentTarget.TryGetComponent<TowerTroop>(out troopHit);

        if (troopHit)
        {
            anim.SetTrigger("Attack Now");

            troopHit.RecieveDMG(enemyDamage);
        }
    }

    private void GetNextWaypoint()
    {
        if(waypointIndex >= waypointsList.Count - 1)
        {
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
                SoundManager.Instance.PlaySoundOneShot(Sounds.EnemyEnterBase);

                playerBase.RecieveDamage(this);
            }

            foreach (Renderer renderer in renderersToFadeOnHitPlayer)
            {
                Helpers.SetMatImmediate(renderer, reachPlayerBaseMat);
                Helpers.GeneralFloatValueTo(gameObject, renderer.material, 1, 0, timeToDieOnReachPlayerBase / GameManager.gameSpeed, LeanTweenType.linear, materialKeyReachBase);
            }

            Destroy(gameObject, timeToDieOnReachPlayerBase / GameManager.gameSpeed);
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

    private void OnDestroy()
    {
        WaveManager.Instance.ChangeEnemyCount(-1);
    }


    public void RecieveDMG(float amount)
    {
        StopAllCoroutines();
        Helpers.SetMatImmediate(rend, hitMat);
        Helpers.GeneralFloatValueTo(gameObject, rend.material, 10, 0, timeToDisplayHit / GameManager.gameSpeed, LeanTweenType.linear, materialKeyGetHit, AfterRecieveDMG);

        enemyHealth -= amount;

        anim.SetTrigger("Get Hit");

        if(enemyHealth <= 0)
        {
            SoundManager.Instance.PlaySoundOneShot(Sounds.EnemyDies);

            RollGiveScrap();
            Instantiate(onDeathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            SoundManager.Instance.PlaySoundOneShot(Sounds.EnemyHit);
        }
    }
    public void BecomeSlowed(float speedMod, float timeToSlow)
    {
        speedModifier = speedMod;

        timeToEndSlow = timeToSlow;

        currentTimeSlowed = timeToEndSlow;
    }

    private void RollGiveScrap()
    {
        int randomNum = UnityEngine.Random.Range(0, 101);

        if(randomNum <= percentageToGiveScrap)
        {
            //if scrap is 30 then we want the number to be 30 to 0, that's 30%;

            Player.Instance.AddResources(ResourceType.scrap, scrapAmount);
        }
    }

    private void AfterRecieveDMG()
    {
        Helpers.GeneralFloatValueTo(gameObject, rend.material, 0, 10, timeToDisplayHit / GameManager.gameSpeed, LeanTweenType.linear, materialKeyGetHit);

        StartCoroutine(Helpers.SetMat(rend, defaultMat, timeToChangeToDefaultMatAfterHit / GameManager.gameSpeed));
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
    public EnemyTypes RetrunEnemyType()
    {
        return enemyType;
    }





    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
