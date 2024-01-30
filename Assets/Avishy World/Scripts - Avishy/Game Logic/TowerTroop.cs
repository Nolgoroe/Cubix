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

    [Header("Preset Data Combat")] 
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float currentAttackCooldown = 0;
    [SerializeField] protected float rotationSpeed = 10;
    [SerializeField] protected float timeToWaitForHealing = 3;
    [SerializeField] protected float currentTimeToWaitForHealing = 0;
    [SerializeField] protected float healingRate = 0.1f;

    [Header("Preset Refs")]
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private Animator anim;


    [Header("Particles Data")]
    [SerializeField] protected GameObject onSpawnParticle;

    [SerializeField] protected float currentHealth = 3;
    protected float originalHealth = 3;
    private float range = 15;
    private float damage = 1;
    bool isDead;
    Vector3 originalScale;

    virtual protected void Start()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        SetRangeIndicator();

        Instantiate(onSpawnParticle, transform);

        LeanTween.scale(gameObject, originalScale, 0.5f).setEase(LeanTweenType.easeOutBounce);
    }

    private void SetRangeIndicator()
    {
        if (rangeIndicator)
        {
            rangeIndicator.localScale = new Vector3(range * 2 / originalScale.x, range * 2 / originalScale.y, range * 2 / originalScale.z);
            rangeIndicator.gameObject.SetActive(false);
        }
    }
    protected virtual void Update()
    {
        if (GameManager.gamePaused) return;

        UpdateTarget();

        if(currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime * GameManager.gameSpeed;
        }


        if(currentTimeToWaitForHealing <= 0)
        {
            currentTimeToWaitForHealing = 0;
            Heal();
        }
        else
        {
            currentTimeToWaitForHealing -= Time.deltaTime * GameManager.gameSpeed;
        }

        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * (rotationSpeed * GameManager.gameSpeed));

        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = attackRate;
        }

    }

    private void Heal()
    {
        currentHealth += healingRate * Time.deltaTime * GameManager.gameSpeed;

        if(currentHealth > originalHealth)
        {
            currentHealth = originalHealth;
        }
    }

    private void Attack()
    {
        EnemyParent enemyHit;
        currentTarget.TryGetComponent<EnemyParent>(out enemyHit);

        if (enemyHit)
        {
            if(anim)
            anim.SetTrigger("Attack Now");

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
        currentHealth = _HP;
        originalHealth = _HP;
        range = _range;
        damage = _dmg;
    }

    public void RecieveDMG(int damage)
    {
        if (isDead) return;

        currentTimeToWaitForHealing = timeToWaitForHealing;

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Recieve dmg");
            Destroy(gameObject);

            connectedTower.LoseTroop(this);

            return;
        }

        currentHealth -= damage;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
