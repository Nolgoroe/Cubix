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
    [SerializeField] protected List<TowerBuffDataHolder> currentTowerBuffs;

    [Header("Visuals")]
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] protected ParticleSystem onSpawnParticle;
    [SerializeField] protected Vector2Int currentCellOnPos;


    protected Vector3 originalScale;


    private void OnEnable()
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
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        GameManager.Instance.AddTowerToRelaventList(this);

        if(!GameGridControls.Instance.rapidControls)
        {
            if (towerDie)
            {
                towerDie.OnRollEndEvent.AddListener(RecieveBuffAfterRoll);
            }
        }

        //spawn effect
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        Instantiate(onSpawnParticle, newPos, Quaternion.identity);

        LeanTween.scale(gameObject, originalScale, 0.5f).setEase(LeanTweenType.easeOutBounce);
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
    }




    public abstract void InitTowerData(Vector2Int positionOfCell, Die connectedDie);
    public abstract void RecieveBuffAfterRoll(Die die);
    public abstract void OnHoverOverOccupyingCell(bool isHover);

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
            towerDie.BackToPlayerArea();
            towerDie.DisplayResources();

            DiceManager.Instance.ResetDiceToWorldList();
            DiceManager.Instance.AddDiceToResources(towerDie);

            GridManager.Instance.ReturnCellAtVector(currentCellOnPos).ResetCellOnStartTurn();
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

    public virtual void CleanTroopsCompletely()
    {

    }
}
