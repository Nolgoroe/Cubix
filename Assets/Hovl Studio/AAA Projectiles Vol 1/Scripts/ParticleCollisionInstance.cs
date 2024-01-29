/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleCollisionInstance : MonoBehaviour
{
    public GameObject[] EffectsOnCollision;
    public float DestroyTimeDelay = 5;
    public bool UseWorldSpacePosition;
    public float Offset = 0;
    public Vector3 rotationOffset = new Vector3(0,0,0);
    public bool useOnlyRotationOffset = true;
    public bool UseFirePointRotation;
    public bool DestoyMainEffect = true;
    [SerializeField] private ParticleSystem[] parts;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem ps;
    ParticleSystem.MainModule control;

    private void Start()
    {
        foreach (ParticleSystem system in parts)
        {
            control = system.main;
            control.simulationSpeed = GameManager.gameSpeed;
        }

        Destroy(gameObject, DestroyTimeDelay + 0.5f);
    }
    void OnParticleCollision(GameObject other)
    {
        TowerBullet bullet;
        TryGetComponent<TowerBullet>(out bullet);
        if (bullet)
            bullet.OnCollidedWithObject(other);

        int numCollisionEvents = parts[0].GetCollisionEvents(other, collisionEvents);     
        for (int i = 0; i < numCollisionEvents; i++)
        {
            foreach (var effect in EffectsOnCollision)
            {
                var instance = Instantiate(effect, collisionEvents[i].intersection + collisionEvents[i].normal * Offset, new Quaternion()) as GameObject;
                if (!UseWorldSpacePosition) instance.transform.parent = transform;
                if (UseFirePointRotation) { instance.transform.LookAt(transform.position); }
                else if (rotationOffset != Vector3.zero && useOnlyRotationOffset) { instance.transform.rotation = Quaternion.Euler(rotationOffset); }
                else
                {
                    instance.transform.LookAt(collisionEvents[i].intersection + collisionEvents[i].normal);
                    instance.transform.rotation *= Quaternion.Euler(rotationOffset);
                }
                Destroy(instance, DestroyTimeDelay);
            }
        }
        if (DestoyMainEffect == true)
        {
            Destroy(gameObject);
        }
    }
}
