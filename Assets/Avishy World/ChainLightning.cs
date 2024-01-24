using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private GameObject chainLightningEffect;
    [SerializeField] private GameObject beenStruck;
    [SerializeField] private int amountToChain = 5;
    [SerializeField] private float damage = 11;
    [SerializeField] private float damageLoss = 2;


    private SphereCollider collider;
    private GameObject startObject;
    private Animator anim;
    private int singleSpawns; // make sure we can't attack 2 enemies at once.
    private GameObject endObject;
    private ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        if(amountToChain == 0)
        {
            Destroy(gameObject);
            return;
        }

        collider = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        particleSystem = GetComponent<ParticleSystem>();

        startObject = gameObject;

        singleSpawns = 1;

        Destroy(gameObject, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(enemyLayer == (enemyLayer | (1<< other.gameObject.layer)) && !other.GetComponentInChildren<EnemyStruck>())
        {
            if(singleSpawns != 0)
            {
                amountToChain --;
                singleSpawns--;
                damage -= damageLoss;

                if(damage <= 0)
                {
                    damage = 1;
                }

                endObject = other.gameObject;


                Instantiate(beenStruck, other.gameObject.transform);
                Instantiate(chainLightningEffect, other.gameObject.transform.position, Quaternion.identity);


                EnemyParent enemyHit;
                other.TryGetComponent<EnemyParent>(out enemyHit);
                if (enemyHit)
                {
                    enemyHit.RecieveDMG(damage);
                }


                anim.StopPlayback();
                collider.enabled = false;


                particleSystem.Play();
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = startObject.transform.position;
                particleSystem.Emit(emitParams, 1);
                emitParams.position = endObject.transform.position;
                particleSystem.Emit(emitParams, 1);
            }
        }
    }
}
