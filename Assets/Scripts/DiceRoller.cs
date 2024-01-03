using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 posConstraintRange;
    [SerializeField] private Vector3 rotationForceMin;
    [SerializeField] private Vector3 rotationForceMax;
    [SerializeField] private float throwForce;
    [SerializeField] private bool constraintX;
    [SerializeField] private bool constraintY;
    [SerializeField] private bool constraintZ;

    private Vector3 _ogPos;

    private void Start()
    {
        _ogPos = transform.position;
    }

    private void LateUpdate()
    {
        if (rb.velocity.magnitude != 0 || rb.angularVelocity.magnitude != 0)
        {
            Contraint();
        }
    }

    private void OnMouseDown()
    {
        Roll();
    }

    private void Roll()
    {
        Vector3 throwVec = new Vector3(0, throwForce, 0);

        Vector3 rotForce = new Vector3();
        rotForce.x = Random.Range(rotationForceMin.x, rotationForceMax.x);
        rotForce.y = Random.Range(rotationForceMin.y, rotationForceMax.y);
        rotForce.z = Random.Range(rotationForceMin.z, rotationForceMax.z);

        rb.AddForce(throwVec, ForceMode.Impulse);
        rb.AddTorque(rotForce, ForceMode.Impulse);
    }

    private void Contraint()
    {
        Vector3 newPos = transform.position;

        if (constraintX)
        {
            float minX, maxX;
            minX = _ogPos.x - posConstraintRange.x;
            maxX = _ogPos.x + posConstraintRange.x;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        }

        if (constraintY)
        {
            float minY, maxY;
            minY = _ogPos.y - posConstraintRange.y;
            maxY = _ogPos.y + posConstraintRange.y;
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        }

        if (constraintZ)
        {
            float minZ, maxZ;
            minZ = _ogPos.z - posConstraintRange.z;
            maxZ = _ogPos.z + posConstraintRange.z;
            newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        }

        transform.position = newPos;
    }
}
