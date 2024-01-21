using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum DieElement { Water, Fire, Poison }
public enum DieType { D6, D8 }

//[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
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
    [SerializeField] private DieType DieType;
    [SerializeField] private TowerBaseParent towerPrefabConnected;
    [SerializeField] private float _reqStagnantTime = 1;
    [SerializeField] private Transform lockTransform;
    [SerializeField] private Outline outline;
    [SerializeField] private Color diceColor;

    private bool _isDragging;
    private bool _isInWorld;
    private float _stagnantTimer;
    [SerializeField] private DieFace _currentTopFace;
    private Vector3 originalPos;
    //private Vector3 originalScale;
    private float timeTillStartDrag = 0.2f;
    private float currentTimeTillStartDrag = 0;
    private Quaternion targetQuat;
    bool isRolling;

    public bool IsRolling { get { return isRolling; } }
    public Rigidbody RB { get { return _rb; } }


    private void Start()
    {
        
        OnRollStartEvent.AddListener(SetMovingTrue);
        OnRollEndEvent.AddListener(OrientCubeToCamrea);
        OnRollEndEvent.AddListener(TransformAfterRoll);

        OnDragStartEvent.AddListener(SetValuesOnDragStart);
        OnDragStartEvent.AddListener(SetMovingFalse);

        OnDragEndEvent.AddListener(SetValuesOnDragEnd);

        OnPlaceEvent.AddListener(SetValuesOnPlacement);
        OnPlaceEvent.AddListener(SetMovingFalse);

        OnDestroyDieEvent.AddListener(OnDestroyDie);

        DisplayResources(); //for build purpose

        originalPos = transform.localPosition;
        //originalScale = transform.localScale;

        diceCam = GameManager.Instance.ReturnDiceCamera();
    }

    public void InitDiceInSlot(Transform _lockTransform, DiceSO diceData)
    {
        lockTransform = _lockTransform;

        towerPrefabConnected = diceData.towerPrefab;
        diceColor = diceData.dieMaterial.color;


        switch (diceData.dieType)
        {
            case DieType.D6:
                SetDiceValue(6, diceData);
                break;
            case DieType.D8:
                SetDiceValue(8, diceData);
                break;
            default:
                break;
        }

    }

    private void SetDiceValue(int amountOfFaces, DiceSO diceData)
    {
        for (int i = 0; i < amountOfFaces; i++)
        {
            faces[i].ChangeFaceMat(diceData.dieMaterial);

            int randomResourceFaceIndex = Random.Range(0, System.Enum.GetValues(typeof(ResourceType)).Length);
            ResourceData resourceData = new ResourceData();
            resourceData.Type = (ResourceType)randomResourceFaceIndex;
            resourceData.Value = Random.Range(1, 10); //temp
            resourceData.Icon = DiceManager.Instance.ReturnIconByType(resourceData.Type);

            faces[i].SetResource(resourceData);
            faces[i].DisplayResource();


            BuffData buffData = new BuffData();
            buffData.Type = diceData.buffDataList[i].Type;
            buffData.Value = diceData.buffDataList[i].Value;
            buffData.Icon = DiceManager.Instance.ReturnIconByType(buffData.Type);
            faces[i].SetBuff(buffData);
        }
    }
    private void LateUpdate()
    {
        CheckState();
    }

    private void OrientCubeToCamrea(Die die)
    {
        isRolling = false;
        DieFaceValue faceValue = die.GetTopValue(); // _currentTopFace value is always set in this function. we call it to be safe that we don't use a null value 

        if (!_isInWorld)
        {
            targetQuat = Quaternion.Euler(die.ReturnCurrentTopFace().ReturnOrientationOnEndRoll());
        }
        else
        {
            Vector3 direction = (GameManager.Instance.ReturnMainCamera().transform.position - transform.position).normalized;
            targetQuat = Quaternion.FromToRotation(Vector3.forward, direction) * Quaternion.Euler(die.ReturnCurrentTopFace().ReturnOrientationOnEndRollWorld());
        }

        RB.isKinematic = true;
    }

    private void TransformAfterRoll(Die die)
    {
        LeanTween.rotate(gameObject, die.targetQuat.eulerAngles, 1f);// speed is temp here

        if(die._isInWorld)
        LeanTween.scale(gameObject, transform.localScale * 2, 0.2f);// speed is temp here
    }

    private void CheckState()
    {
        if (isRolling)
        {
            if (_rb.velocity.magnitude < 0.001f)
            {
                _stagnantTimer += Time.deltaTime;

                if (_stagnantTimer >= _reqStagnantTime)//if die stands still for more than x seconds apply rndRoll logic
                {
                    isRolling = false;
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
        RB.isKinematic = false;

        Debug.Log("Roll started");
        isRolling = true;
    }
    private void SetMovingFalse()
    {
        Debug.Log("Roll started");
        isRolling = false;
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

        UIManager.Instance.DisplayDiceFacesUI(false, this);
    }
    private void OnMouseEnter()
    {
        UIManager.Instance.DisplayDiceFacesUI(true, this);
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
    {
        RB.isKinematic = true;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // temp here

        GameGridControls.Instance.SetCurrentDieDragging(this);

        ChangeLayerRecursive(transform, "Default");

    }
    private void SetValuesOnDragEnd()
    {
        //called if we stopped dragging and DIDN'T place a tower.
        RB.isKinematic = false;
        transform.localScale = new Vector3(1, 1, 1); // temp here

        transform.localPosition = originalPos;

        ChangeLayerRecursive(transform, "Dice");
    }

    private void SetValuesOnPlacement()
    {
        DisplayBuffs();
        RB.isKinematic = false;
        _isInWorld = true;
        ChangeLayerRecursive(transform, "Default");
    }

    private void ChangeLayerRecursive(Transform trans, string nameOfLayer)
    {

        //the string nameOfLayer might change to layermask or even int of layer

        foreach (Transform child in trans)
        {
            child.gameObject.layer = LayerMask.NameToLayer(nameOfLayer);
            ChangeLayerRecursive(child, nameOfLayer);
        }
    }

    private void OnDestroyDie()
    {

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
        lockTransform.gameObject.SetActive(isLocking ? true : false);
    }

    public GameObject ReturnTowerPrefab()
    {
        return towerPrefabConnected.gameObject;
    }

    public bool ReturnInWorld()
    {
        return _isInWorld;
    }
    public Color ReturnDiceColor()
    {
        return diceColor;
    }
    public DieType ReturnDieType()
    {
        return DieType;
    }

    public DieFace ReturnCurrentTopFace()
    {
        return _currentTopFace;
    }

    public void ResetTransformData()
    {
        transform.localScale = new Vector3(0.5f,0.5f,0.5f); //temp here
    }
}

