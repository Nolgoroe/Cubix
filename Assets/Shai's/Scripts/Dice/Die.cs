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


    [Header("Dice Data")]
    [SerializeField] private DieType DieType;
    [SerializeField] private DieElement element;
    [SerializeField] private Material diceMat;
    [SerializeField] private bool isLocked;

    [Header("Dice Transform Data")]
    [SerializeField] private Vector3 scaleOnDrag = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 scaleInPlayerBase = Vector3.one;

    [Header("Faces")]
    [SerializeField] private List<DieFace> faces;
    [SerializeField] private DieFace _currentTopFace;

    [Header("Tower Connected")]
    [SerializeField] private TowerBaseParent towerPrefabConnected;
    [SerializeField] private TowerBaseParent currentTowerParent;
    [SerializeField] private bool specialAttackUnlcoked;

    [Header("Roll Data")]
    [SerializeField] private float _reqStagnantTime = 1;

    [Header("References")]
    [SerializeField] private Camera diceCam;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform lockTransform;
    [SerializeField] private Outline outline;
    [SerializeField] private Transform rangeIndicator;
    [SerializeField] private Transform originalParent;


    private bool _isDragging;
    private bool _isInWorld;
    private bool isRolling;

    private float timeTillStartDrag = 0.2f;
    private float currentTimeTillStartDrag = 0;
    private float _stagnantTimer;

    private Vector3 originalPos;
    private Quaternion targetQuat;

    private DieRoller roller;

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

        SetRangeIndicator();

        roller = GetComponent<DieRoller>();
    }
    private void LateUpdate()
    {
        CheckState();
    }

    private void SetRangeIndicator()
    {
        //radius is half of the diameter of a circle
        if (rangeIndicator)
        {
            float range = 0;
            Vector3 originalScale = Vector3.one;

            switch (towerPrefabConnected)
            {
                case RangeTowerParentScript rangeTower:
                    range = rangeTower.ReturnRangeTower();

                    originalScale = scaleOnDrag;
                    //originalScale = rangeTower.ReturnOriginalTowerScale();
                    break;
                default:
                    break;
            }


            rangeIndicator.localScale = new Vector3(range * 2 / originalScale.x, range * 2 / originalScale.y, range * 2 / originalScale.z);
            rangeIndicator.gameObject.SetActive(false);
        }
    }

    private void SetDiceValueSpecific(int amountOfFaces, DieData diceData)
    {
        for (int i = 0; i < amountOfFaces; i++)
        {
            faces[i].ChangeFaceMat(diceData.material);

            int randomResourceFaceIndex = Random.Range(0, System.Enum.GetValues(typeof(ResourceType)).Length);
            ResourceData resourceData = new ResourceData();
            resourceData.Type = (ResourceType)randomResourceFaceIndex;
            resourceData.Value = Random.Range(1, 10); //temp
            resourceData.Icon = DiceManager.Instance.ReturnIconByType(resourceData.Type);

            faces[i].SetResource(resourceData);


            BuffData buffData = new BuffData();
            buffData.Type = diceData.facesValues[i].Buff.Type;
            buffData.Value = diceData.facesValues[i].Buff.Value;
            buffData.Icon = DiceManager.Instance.ReturnIconByType(buffData.Type);
            faces[i].SetBuff(buffData);

            faces[i].DisplayResource();
        }
    }
    private void SetDiceValueRandom(int amountOfFaces, DiceSO diceData)
    {
        for (int i = 0; i < amountOfFaces; i++)
        {
            faces[i].ChangeFaceMat(diceData.dieMaterial);

            int randomResource = Random.Range(0, System.Enum.GetValues(typeof(ResourceType)).Length);
            ResourceData resourceData = new ResourceData();
            resourceData.Type = (ResourceType)randomResource;
            resourceData.Value = Random.Range(1, 10); //temp
            resourceData.Icon = DiceManager.Instance.ReturnIconByType(resourceData.Type);

            faces[i].SetResource(resourceData);

            int randomBuff = Random.Range(0, System.Enum.GetValues(typeof(BuffType)).Length);
            BuffData buffData = new BuffData();
            buffData.Type = (BuffType)randomBuff;
            buffData.Value = Random.Range(1, 10); //temp
            buffData.Icon = DiceManager.Instance.ReturnIconByType(buffData.Type);
            faces[i].SetBuff(buffData);


            faces[i].DisplayResource();
        }
    }

    private void OrientCubeToCamrea(Die die)
    {
        isRolling = false;
        DieFaceValue faceValue = die.GetTopValue(); // _currentTopFace value is always set in this function. we call it to be safe that we don't use a null value 

        targetQuat = Quaternion.Euler(die.ReturnCurrentTopFace().ReturnOrientationOnEndRoll());

        RB.isKinematic = true;
    }

    private void TransformAfterRoll(Die die)
    {
        LeanTween.rotate(gameObject, die.targetQuat.eulerAngles, 0.2f).setOnComplete(TowerRotate);// speed is temp here

        if (die._isInWorld)
            LeanTween.scale(gameObject, transform.localScale * 2, 0.2f);// speed is temp here
    }

    private void TowerRotate()
    {
        if (currentTowerParent)
            currentTowerParent.RotateTowardsCameraEndRoll();
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
                    //Debug.Log("Roll ended");
                }
            }
            else if (_stagnantTimer > 0)
            {
                _stagnantTimer = 0;
            }
        }
    }

    private void SetMovingTrue()
    {
        RB.isKinematic = false;

        //Debug.Log("Roll started");
        isRolling = true;
    }
    private void SetMovingFalse()
    {
        //Debug.Log("Roll ended");
        isRolling = false;
    }

    private void OnMouseDrag()
    {
        if (isLocked || !GameManager.playerTurn) return;
        currentTimeTillStartDrag += Time.deltaTime;

        if (currentTimeTillStartDrag >= timeTillStartDrag)
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
        if (!GameManager.playerTurn) return;

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

        if (rangeIndicator)
        {
            rangeIndicator.gameObject.SetActive(true);
        }

        transform.localScale = scaleOnDrag;

        GameGridControls.Instance.SetCurrentDieDragging(this);

        GridManager.Instance.ToggleAllRelaventSlots(towerPrefabConnected.ReturnRequiresPathCells());

        ChangeLayerRecursive(transform, "Default");


    }
    private void SetValuesOnDragEnd()
    {
        if (rangeIndicator)
        {
            rangeIndicator.gameObject.SetActive(false);
        }

        //called if we stopped dragging and DIDN'T place a tower.
        RB.isKinematic = false;
        transform.localScale = scaleInPlayerBase;

        transform.localPosition = originalPos;

        GridManager.Instance.ActivateAllTowerBaseCells();

        ChangeLayerRecursive(transform, "Dice");
    }

    private void SetValuesOnPlacement()
    {
        if (rangeIndicator)
        {
            rangeIndicator.gameObject.SetActive(false);
        }

        DisplayBuffs();
        RB.isKinematic = false;
        _isInWorld = true;


        GridManager.Instance.ActivateAllTowerBaseCells();

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






    public void InitDiceInSlot(Transform _lockTransform, DieData diceData)
    {
        lockTransform = _lockTransform;

        towerPrefabConnected = diceData.towerPrefabConnected;
        diceMat = diceData.material;


        switch (diceData.dieType)
        {
            case DieType.D6:
                SetDiceValueSpecific(6, diceData);
                break;
            case DieType.D8:
                SetDiceValueSpecific(8, diceData);
                break;
            default:
                break;
        }

        originalParent = transform.parent;

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

    public void InitDie(TowerBaseParent tower)
    {
        currentTowerParent = tower;
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
        return diceMat.color;
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
        transform.localScale = scaleOnDrag;
    }
    public void BackToPlayerArea()
    {
        if (rangeIndicator)
        {
            rangeIndicator.gameObject.SetActive(false);
        }

        RB.isKinematic = false;
        RB.velocity = Vector3.zero;
        transform.SetParent(originalParent);
        transform.localPosition = originalPos;
        _isInWorld = false;

        roller.SetOGPos(transform);
        transform.localScale = scaleInPlayerBase;

        ChangeLayerRecursive(transform, "Dice");

    }
    public TowerBaseParent ReturnCurrentTowerParent()
    {
        return currentTowerParent;
    }
    public DieRoller ReturnDieRoller()
    {
        return roller;
    }

    public bool ReturnIsLocked()
    {
        return isLocked;
    }

    public DieData ExportTransferData()
    {
        DieData data = new DieData();

        data.dieType = DieType;
        data.element = element;
        data.material = diceMat;

        List<DieFaceValue> tmpFaceValues = new List<DieFaceValue>();
        foreach (var face in faces)
        {
            tmpFaceValues.Add(face.GetFaceValue());
        }
        data.facesValues = tmpFaceValues;

        data.towerPrefabConnected = towerPrefabConnected;

        return data;
    }

    public void ImportTransferData(DieData data)
    {
        DieType = data.dieType;
        element = data.element;
        diceMat = data.material;

        for (int i = 0; i < data.facesValues.Count; i++)
        {
            faces[0].SetBuff(data.facesValues[i].Buff);
            faces[0].SetResource(data.facesValues[i].Resource);
        }

        towerPrefabConnected = data.towerPrefabConnected;
    }

    public bool ReturnSpecialAttackUnlcoked()
    {
        return specialAttackUnlcoked;
    }

    public DieElement ReturnDieElement()
    {
        return element;
    }
}



