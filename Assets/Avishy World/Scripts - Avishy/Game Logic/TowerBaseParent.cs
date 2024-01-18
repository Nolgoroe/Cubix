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


    protected Vector3 originalScale;
    virtual protected void Start()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        if (towerDie)
        {
            towerDie.OnRollEndEvent.AddListener(RecieveBuffAfterRoll);
            //towerDie.OnRollEndEvent.AddListener(DisplayBuffAfterRoll);
        }

        GameManager.Instance.AddTowerToRelaventList(this);

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
    //protected void SpawnBuffCubeOnCreation()
    //{
    //    //this switch is temp
    //    //switch (towerDie.ReturnDieType())
    //    //{
    //    //    case DieType.D6:
    //    //        resultDiceHolder.localRotation = Quaternion.Euler(new Vector3(45, 0, 0));
    //    //        break;
    //    //    case DieType.D8:
    //    //        resultDiceHolder.localRotation = Quaternion.Euler(new Vector3(7,45,7));
    //    //        break;
    //    //    default:
    //    //        break;
    //    //}

    //    if(towerDie)
    //    {
    //        GameObject go =  Instantiate(towerDie.gameObject, resultDiceHolder);
    //        go.transform.localPosition = Vector3.zero;
    //        go.transform.localRotation = Quaternion.Euler(Vector3.zero);

    //        foreach (Component comp in go.GetComponents<Component>())
    //        {
    //            if (!(comp is Transform))
    //            {
    //                Destroy(comp);
    //            }
    //        }

    //        ResultDiceDisplay diceDisplay = go.AddComponent<ResultDiceDisplay>();
    //        diceDisplay.InitDiceDisplay(towerDie);
    //        diceDisplay.GetCameraFacingValue();

    //        resultDiceDisplay = diceDisplay;
    //    }

    //    resultDiceHolder.gameObject.SetActive(false);
    //}
    public abstract void InitTowerData(Vector2Int positionOfCell, Die connectedDie);
    public abstract void RecieveBuffAfterRoll(Die die);
    //protected void DisplayBuffAfterRoll(Die die)
    //{
    //    DieFaceValue dieFaceValue = die.GetTopValue();

    //    resultDiceHolder.gameObject.SetActive(true);

    //    resultDiceDisplay.SetFaceData(dieFaceValue.Buff);
    //    towerDie.gameObject.SetActive(false);
    //}
    public abstract void OnHoverOverOccupyingCell(bool isHover);

    public CellTypeColor ReturnCellColorType()
    {
        return requiredCellColorType;
    }

    public List<TowerBuffDataHolder> ReturnTowerBuffList()
    {
        return currentTowerBuffs;
    }

    public void OnStartPlayerTurn()
    {
        towerDie.gameObject.SetActive(true);

    }
    public void OnEndPlayerTurn()
    {
        towerDie.ResetTransformData();
        towerDie.gameObject.SetActive(false);
        resultDiceHolder.gameObject.SetActive(false);
    }

    public Color ReturnTowerRequiredColor()
    {
        return towerRequiredColor;
    }
    public Transform ReturnResultDiceTransform()
    {
        return resultDiceHolder;
    }
}
