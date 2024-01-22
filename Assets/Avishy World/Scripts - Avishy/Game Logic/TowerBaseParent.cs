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
    [SerializeField] protected Vector2Int currentCellOnPos;
    [SerializeField] protected CellTypeColor requiredCellColorType;
    [SerializeField] protected Color towerRequiredColor;
    [SerializeField] protected Die towerDie;
    //[SerializeField] protected ResultDiceDisplay resultDiceDisplay;
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] protected Transform resultDiceHolder;
    [SerializeField] protected List<TowerBuffDataHolder> currentTowerBuffs;

    [SerializeField] protected ParticleSystem onSpawnParticle;

    [SerializeField] protected bool requiresPathCells;


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
            //GameManager.Instance.ClearTowerToRelaventList();
            DiceManager.Instance.ResetDiceToWorld();
            DiceManager.Instance.AddDiceToResources(towerDie.ReturnDieRoller());

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
        Vector3 direction = GameManager.Instance.ReturnMainCamera().transform.position - towerDie.ReturnCurrentTopFace().transform.position;

        Debug.DrawLine(resultDiceHolder.position, direction * 1000, Color.red, Mathf.Infinity);
        Quaternion lookAt = Quaternion.LookRotation(direction);
        LeanTween.rotate(resultDiceHolder.gameObject, lookAt.eulerAngles, 0.2f);
    }


    public Color ReturnTowerRequiredColor()
    {
        return towerRequiredColor;
    }
    public Transform ReturnResultDiceTransform()
    {
        return resultDiceHolder;
    }
    public bool ReturnRequiresPathCells()
    {
        return requiresPathCells;
    }

    public virtual void CleanTroopsCompletely()
    {

    }

    //public Vector3 ReturnOriginalTowerScale()
    //{
    //    return originalScale;
    //}
}
