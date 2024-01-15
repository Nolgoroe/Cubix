using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum DieElement { Water, Fire, Poison }

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class Die : MonoBehaviour
{
    public UnityEvent OnRollStartEvent;
    public UnityEvent<Die> OnRollEndEvent;
    public UnityEvent OnDragStartEvent;
    public UnityEvent OnDragEndEvent;
    public UnityEvent OnPlaceEvent;
    public UnityEvent OnDestroyDieEvent;

    public bool isLocked;
    [SerializeField] private Camera diceCam;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private List<DieFace> faces;
    [SerializeField] private DieElement element;
    [SerializeField] private TowerBaseParent towerPrefabConnected;        //new avishy
    [SerializeField] private float _reqStagnantTime = 1;
    [SerializeField] private Outline outline;
    [SerializeField] private SpriteRenderer lockRenderer;

    private bool _isMoving;
    private bool _isDragging;
    private bool _isInWorld;
    private float _stagnantTimer;
    private DieFace _currentTopFace;
    private Vector3 originalPos;
    private float timeTillStartDrag = 0.2f;
    private float currentTimeTillStartDrag = 0;

    public bool IsMoving { get { return _isMoving; } }
    public Rigidbody RB { get { return _rb; } }


    private void Start()
    {
        //new avishy
        OnRollStartEvent.AddListener(SetMovingTrue);

        OnDragStartEvent.AddListener(SetValuesOnDragStart);

        OnDragEndEvent.AddListener(SetValuesOnDragEnd);

        OnPlaceEvent.AddListener(SetValuesOnPlacement);

        OnDestroyDieEvent.AddListener(OnDestroyDie);

        DisplayResources();//for build purpose

        originalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (_isMoving)
        {
            if (_rb.velocity.magnitude < 0.001f)
            {
                _stagnantTimer += Time.deltaTime;

                if (_stagnantTimer >= _reqStagnantTime)//if die stands still for more than x seconds apply rndRoll logic
                {
                    _isMoving = false;
                    OnRollEndEvent?.Invoke(this);
                    _stagnantTimer = 0;
                    GetTopValue();
                    Debug.Log("Roll ended");
                }
            }
            else if(_stagnantTimer > 0)
            {
                _stagnantTimer = 0;
            }
        }
    }

    private void SetMovingTrue()
    {
        Debug.Log("Roll started");
        _isMoving = true;
    }

    public DieFaceValue GetTopValue()
    {
        float lowestAngle = float.MaxValue;
        Vector3 tmpFaceVec;
        float tmpAngle;
        foreach (var face in faces)//find the face with the angle closest to up
        {
            tmpFaceVec = face.transform.position - transform.position;
            tmpAngle = Vector3.Angle(Vector3.up, tmpFaceVec);

            if (tmpAngle < lowestAngle)
            {
                lowestAngle = tmpAngle;
                _currentTopFace = face;
            }
        }
        AdjustRotation();

        return _currentTopFace.GetFaceValue();
    }

    public DieFace[] GetAllFaces()
    {
        return faces.ToArray();
    }

    [ContextMenu("DisplayBuffs")]
    public void DisplayBuffs()
    {
        foreach (var face in faces)
        {
            face.DisplayBuff();
        }
    }

    [ContextMenu("DisplayResources")]
    public void DisplayResources()
    {
        foreach (var face in faces)
        {
            face.DisplayResource();
        }
    }

    private void OnMouseDrag()
    {
        if (isLocked) return;
        currentTimeTillStartDrag += Time.deltaTime;

        if(currentTimeTillStartDrag >= timeTillStartDrag)
        {
            OnDragStartEvent?.Invoke();
            _isDragging = true;
        }
    }

    private void OnMouseOver()
    {
        outline.SetOutlineMode(Outline.Mode.OutlineVisible);
    }

    private void OnMouseExit()
    {
        outline.SetOutlineMode(Outline.Mode.OutlineHidden);
    }

    private void OnMouseUp()
    {
        currentTimeTillStartDrag = 0;
        if (_isDragging)
        {
            _isDragging = false;

            return;
        }

        if (isLocked)
        {
            LockDie(false);
        }
        else
        {
            LockDie(true);
        }

        //select die for locking here
        Debug.Log("Mouse up");
    }

    private void AdjustRotation()
    {
        //float tmpAngle = 180 - Vector3.Angle(Vector3.back, _currentTopFace.transform.up * -1);
        //transform.Rotate(Vector3.up * tmpAngle, Space.World);

    }



    private void SetValuesOnDragStart()
    {        //new avishy

        RB.isKinematic = true;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // temp here

        GameGridControls.Instance.SetCurrentDieDragging(this);

        ChangeLayerRecursive(transform, "Default");

    }
    private void SetValuesOnDragEnd()
    {        //new avishy

        RB.isKinematic = false;
        transform.localScale = new Vector3(1, 1, 1); // temp here

        transform.localPosition = originalPos;

        ChangeLayerRecursive(transform, "Dice");
    }

    private void SetValuesOnPlacement()
    {        //new avishy
        DisplayBuffs();
        RB.isKinematic = false;
        _isInWorld = true;
        ChangeLayerRecursive(transform, "Default");
    }

    private void ChangeLayerRecursive(Transform trans, string nameOfLayer)
    {        //new avishy

        //the string nameOfLayer might change to layermask or even int of layer

        foreach (Transform child in trans)
        {
            child.gameObject.layer = LayerMask.NameToLayer(nameOfLayer);
            ChangeLayerRecursive(child, nameOfLayer);
        }
    }

    private void OnDestroyDie()
    {        //new avishy

        OnRollStartEvent.RemoveAllListeners();
        OnRollEndEvent.RemoveAllListeners();
        OnDragStartEvent.RemoveAllListeners();
        OnDragEndEvent.RemoveAllListeners();
        OnPlaceEvent.RemoveAllListeners();
        OnDestroyDieEvent.RemoveAllListeners();

        Destroy(gameObject);
    }



    public void LockDie(bool isLocking)
    {
        isLocked = isLocking ? true : false;
        lockRenderer.gameObject.SetActive(isLocking ? true : false);
    }

    public GameObject ReturnTowerPrefab()
    {        //new avishy

        return towerPrefabConnected.gameObject;
    }

    public bool ReturnInWorld()
    {
        return _isInWorld;
    }
}

