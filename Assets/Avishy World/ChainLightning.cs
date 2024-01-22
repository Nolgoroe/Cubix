using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    private SphereCollider collider;
    public LayerMask enemyLayer;

    public GameObject chainLightningEffect;
    public GameObject beenStruck;
    public int amountToChain = 5;
    public float damage = 11;
    public float damageLoss = 2;

    private GameObject startObject;
    public GameObject endObject;
    private Animator anim;
    public ParticleSystem particleSystem;
    private int singleSpawns;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if(enemyLayer == (enemyLayer | (1<< other.gameObject.layer)) && !other.GetComponentInChildren<EnemyStruck>())
        {
            if(singleSpawns != 0)
            {
                endObject = other.gameObject;
                amountToChain -= 1;
                damage -= damageLoss;
                Instantiate(chainLightningEffect, other.gameObject.transform.position, Quaternion.identity);
                Instantiate(beenStruck, other.gameObject.transform);

                EnemyParent enemyHit;
                other.TryGetComponent<EnemyParent>(out enemyHit);

                if (enemyHit)
                {
                    enemyHit.RecieveDMG(damage);
                }

                anim.StopPlayback();
                collider.enabled = false;
                singleSpawns --;

                particleSystem.Play();

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = startObject.transform.position;

                particleSystem.Emit(emitParams, 1);

                emitParams.position = endObject.transform.position;

                particleSystem.Emit(emitParams, 1);



                Destroy(gameObject, 1);
            }
        }
    }
}
