using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerBuffDataHolder
{
    public BuffType buffType;
    public Sprite buffIconSprite;
    public Color bgColor;
    public float amount;
}
public abstract class TowerBaseParent : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] protected bool requiresPathCells;
    [SerializeField] protected CellTypeColor requiredCellColorType;

    [Header("Dice Data")]
    [SerializeField] protected Die towerDie;
    [SerializeField] protected Transform resultDiceHolder;
    [SerializeField] private Quaternion originalDiceHolderRotation;
    [SerializeField] protected List<TowerBuffDataHolder> currentTowerBuffs;

    [Header("Combat")]
    [SerializeField] protected float range = 15;
    [SerializeField] protected bool specialAttackUnlocked = false;

    [Header("Stamina System")]
    [SerializeField] protected bool isDisabled = false;
    [SerializeField] protected bool isBeingDragged = false;
    [SerializeField] protected float timeToEnable = 5;
    [SerializeField] protected float currentTimeToEnable = 5;
    [SerializeField] protected GridCell targetCell;

    [Header("Visuals")]
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] protected ParticleSystem onSpawnParticle;
    [SerializeField] protected Vector2Int currentCellOnPos;


    protected Vector3 originalScale;


    protected virtual void OnEnable()
    {
        if (GameGridControls.Instance.rapidControls)
        {
            if (towerDie)
            {
                towerDie.OnRollEndEvent.AddListener(RecieveBuffAfterRoll);
            }
        }
    }
    private void OnDisable()
    {
        if (GameGridControls.Instance.rapidControls)
        {
            if (towerDie)
            {
                towerDie.OnRollEndEvent.RemoveListener(RecieveBuffAfterRoll);
            }
        }
    }
    virtual protected void Start()
    {
        SoundManager.Instance.PlaySoundOneShot(Sounds.PlacingTower);

        originalDiceHolderRotation = resultDiceHolder.rotation;
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        GameManager.Instance.AddTowerToRelaventList(this);
        
        if (towerDie)
        {
            towerDie.OnRollEndEvent.AddListener(RecieveBuffAfterRoll);
        }

        //spawn effect
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        Instantiate(onSpawnParticle, newPos, Quaternion.identity);

        LeanTween.scale(gameObject, originalScale, 0.5f).setEase(LeanTweenType.easeOutBounce);
    }

    virtual protected void Update()
    {
        if (isBeingDragged) return;

        if(isDisabled)
        {
            currentTimeToEnable -= Time.deltaTime * GameManager.gameSpeed;

            if(currentTimeToEnable <= 0)
            {
                SetAsDisabled(false);
                InitTowerData(currentCellOnPos, towerDie);
            }

            return;
        }
    }

    protected void AddNewTowerBuff(DieFaceValue diceFaceValue, Die die)
    {
        if (diceFaceValue.Buff.Type == BuffType.None) return;

        TowerBuffDataHolder holder = new TowerBuffDataHolder();
        holder.buffType = diceFaceValue.Buff.Type;
        holder.bgColor = die.ReturnDiceColor();
        holder.buffIconSprite = diceFaceValue.Buff.Icon;
        holder.amount = diceFaceValue.Buff.Value;

        currentTowerBuffs.Add(holder);

        SoundManager.Instance.PlaySoundOneShot(Sounds.TowerRecieveBuff);
    }

    protected void SetRangeIndicator()
    {
        //radius is half of the diameter of a circle
        if (rangeIndicator)
        {
            rangeIndicator.localScale = new Vector3(range * 2 / originalScale.x, range * 2 / originalScale.y, range * 2 / originalScale.z);
            rangeIndicator.gameObject.SetActive(false);
        }
    }



    public abstract void InitTowerData(Vector2Int positionOfCell, Die connectedDie);
    public abstract void RecieveBuffAfterRoll(Die die);
    public abstract void RecieveRandomBuff(Die die);

    public abstract void OnHoverOverOccupyingCell(bool isHover);
    public abstract List<string> DisplayTowerStats();

    public CellTypeColor ReturnCellColorType()
    {
        return requiredCellColorType;
    }

    public List<TowerBuffDataHolder> ReturnTowerBuffList()
    {
        return currentTowerBuffs;
    }

    public IEnumerator OnStartPlayerTurn()
    {
        if(GameGridControls.Instance.rapidControls)
        {
            resultDiceHolder.gameObject.SetActive(true);
            towerDie.gameObject.SetActive(true);

            yield return new WaitForSeconds(7); // temp time

            UIManager.Instance.DisplayTowerBuffData(false, this);

            towerDie.BackToPlayerArea();
            towerDie.DisplayResources();

            DiceManager.Instance.ResetDiceToWorldList();
            DiceManager.Instance.AddDiceToResources(towerDie);

            GridManager.Instance.ReturnCellAtVector(currentCellOnPos).ResetCell();
            gameObject.SetActive(false);
            CleanTroopsCompletely();
        }
        else
        {
            resultDiceHolder.gameObject.SetActive(true);
            towerDie.gameObject.SetActive(true);
        }
    }

    public void OnEndPlayerTurn()
    {
        resultDiceHolder.rotation = originalDiceHolderRotation;

        towerDie.ResetTransformData();
        towerDie.gameObject.SetActive(false);
        resultDiceHolder.gameObject.SetActive(false);
    }

    public void RotateTowardsCameraEndRoll()
    {
        Vector3 offset = Vector3.zero;
        switch (towerDie.ReturnDieType())
        {
            case DieType.D6:
                break;
            case DieType.D8:
                offset = new Vector3(0,25,0);
                break;
            default:
                break;
        }
        Vector3 direction = (GameManager.Instance.ReturnMainCamera().transform.position + offset) - towerDie.ReturnCurrentTopFace().transform.position;

        Debug.DrawLine(resultDiceHolder.position, direction * 1000, Color.red, Mathf.Infinity);
        Quaternion lookAt = Quaternion.LookRotation(direction);
        LeanTween.rotate(resultDiceHolder.gameObject, lookAt.eulerAngles, 0.2f);
    }


    public bool ReturnRequiresPathCells()
    {
        return requiresPathCells;
    }


    public void SetAsDisabled(bool disabled)
    {
        currentTimeToEnable = timeToEnable;
        isDisabled = disabled;

        towerDie.ManualSetOGPos(towerDie.transform);
        CleanTroopsCompletely();
    }
    public void SetAsBeingDragged(bool isDragged)
    {
        isBeingDragged = isDragged;
    }


    public void ManualSetTowerOnCell(Vector2Int position)
    {
        currentCellOnPos = position;
    }


    protected virtual void CleanTroopsCompletely()
    {

    }

}
