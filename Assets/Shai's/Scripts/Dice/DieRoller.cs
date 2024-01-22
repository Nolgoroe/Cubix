using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private Die die;
    [SerializeField] private Vector3 posConstraintRange;
    [SerializeField] private Vector3 rotationForceMin;
    [SerializeField] private Vector3 rotationForceMax;
    [SerializeField] private float throwForce;
    [SerializeField] private float tpHeight;
    [SerializeField] private bool constraintX;
    [SerializeField] private bool constraintY;
    [SerializeField] private bool constraintZ;

    [SerializeField] private Vector3 massCenter;


    private Vector3 _ogPos;

    private void Start()
    {        

        _ogPos = transform.position;

        die.OnDragStartEvent.AddListener(OnConnectedDieStartDragging);
        die.OnDragEndEvent.AddListener(OnConnectedDieEndDragging);
        die.OnPlaceEvent.AddListener(OnConnectedDiePlace);

        DiceManager.Instance.AddDiceToResources(this);
    }

    private void LateUpdate()
    {
        if (die.RB.velocity.magnitude != 0 || die.RB.angularVelocity.magnitude != 0)
        {
            Contraint();
        }
    }

    public void Roll()
    {
        die.RB.velocity = Vector3.zero;

        if (!die.isLocked && isActiveAndEnabled)
        {
            die.OnRollStartEvent.Invoke();
            die.RB.ResetCenterOfMass();
            StartCoroutine(ChangeMassAtTop());//Change mass only on apex of throw to look more organic
            die.transform.position += Vector3.up * tpHeight;
            Vector3 throwVec = new Vector3(0, throwForce, 0);

            Vector3 rotForce = new Vector3();
            rotForce.x = Random.Range(rotationForceMin.x, rotationForceMax.x);
            rotForce.y = Random.Range(rotationForceMin.y, rotationForceMax.y);
            rotForce.z = Random.Range(rotationForceMin.z, rotationForceMax.z);

            die.RB.AddForce(throwVec, ForceMode.Impulse);
            die.RB.AddTorque(rotForce, ForceMode.Impulse);
        }
    }

    private void Contraint()//constraint selected axis's to a range within origin position
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

    private IEnumerator ChangeMassAtTop()
    {
        yield return new WaitUntil(() => die.RB.velocity.y < 0 && die.IsRolling);
        die.RB.centerOfMass = massCenter;
        //Debug.Log("change mass");
    }

    private void OnConnectedDieStartDragging()
    {       
        constraintX = false;
        constraintY = false;
        constraintZ = false;
    }
    private void OnConnectedDieEndDragging()
    {        
        constraintX = true;
        constraintY = false;
        constraintZ = true;
    }
    private void OnConnectedDiePlace()
    {
        DiceManager.Instance.RemoveDiceToResources(this);
        DiceManager.Instance.AddDiceToWorld(this);

        //maybe better way?
        Vector3 pos = GameGridControls.Instance.ReturnCurrentCell().transform.position;

        _ogPos = pos;

        Vector3 offset = new Vector3(0, 4, 0); //temp
        transform.position = transform.position + offset;

        //for now it's the same as end dragging but we might want extra logic here so I seperated it.
        constraintX = true;
        constraintY = false;
        constraintZ = true;

        gameObject.SetActive(false);
    }

    public void SetOGPos(Transform _transform)
    {
        _ogPos = _transform.position;
    }
}


