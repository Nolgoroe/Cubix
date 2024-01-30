using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Player player;
    [SerializeField] Transform shotOrigin;

    [Header("Combat")]
    [SerializeField] float damage;
    [SerializeField] float bulletsPerShot = 1, TimeBetweenBullets, TimeBetweenShots, range;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool allowHold;
    [SerializeField] LayerMask layersToHit;

    [Header("Effects")]
    [SerializeField] private GameObject shotEffect;

    float currentBulletsPerShot;
    bool isReadyToShoot;
    bool isShooting;

    private void Start()
    {
        isReadyToShoot = true;

        player = Player.Instance;
    }
    private void Update()
    {
        if (!PlayerWeaponManager.isUsingWeapon || EventSystem.current.IsPointerOverGameObject()) return;

        if (allowHold)
        {
            isShooting = Input.GetMouseButton(0);
        }
        else
        {
            isShooting = Input.GetMouseButtonDown(0);
        }

        if(isReadyToShoot && isShooting)
        {
            currentBulletsPerShot = bulletsPerShot;
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        isReadyToShoot = false;
        StartCoroutine(ResetShot());

        for (int i = 0; i < bulletsPerShot; i++)
        {
            if (!player.ReturnHasScrap())
            {
                isReadyToShoot = true;
                StopAllCoroutines();
                yield break;
            }

            player.AddResources(ResourceType.scrap, -1);

            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameManager.Instance.ReturnMainCamera().transform.position.y);
            Ray ray = GameManager.Instance.ReturnMainCamera().ScreenPointToRay(screenPos);
            RaycastHit rayHit;

            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, Mathf.Infinity);
            if (Physics.Raycast(ray, out rayHit, range, layersToHit))
            {
                transform.LookAt(rayHit.point, Vector3.forward);
                GameObject shot = Instantiate(shotEffect, shotOrigin.position, shotOrigin.rotation);
                 
                EnemyParent enemy;

                rayHit.transform.TryGetComponent<EnemyParent>(out enemy);

                if (enemy)
                {
                    enemy.RecieveDMG(damage);
                }
            }

            currentBulletsPerShot--;

            yield return new WaitForSeconds(TimeBetweenBullets);
        }


    }

    private IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(TimeBetweenShots);

        isReadyToShoot = true;
    }
}
