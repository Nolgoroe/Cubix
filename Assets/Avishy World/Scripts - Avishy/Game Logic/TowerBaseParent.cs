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
    [SerializeField] protected Die towerDie;
    [SerializeField] protected ResultDiceDisplay resultDiceDisplay;
    [SerializeField] protected Transform rangeIndicator;
    [SerializeField] protected Transform resultDiceHolder;
    [SerializeField] protected List<TowerBuffDataHolder> currentTowerBuffs;


    virtual protected void Start()
    {
        if (towerDie)
        {
            towerDie.OnRollEndEvent.AddListener(RecieveBuffAfterRoll);
            towerDie.OnRollEndEvent.AddListener(DisplayBuffAfterRoll);
        }

        GameManager.Instance.AddTowerToRelaventList(this);
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
    protected void SpawnBuffCubeOnCreation()
    {
        if(towerDie)
        {
            GameObject go =  Instantiate(towerDie.gameObject, resultDiceHolder);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            foreach (Component comp in go.GetComponents<Component>())
            {
                if (!(comp is Transform))
                {
                    Destroy(comp);
                }
            }

            ResultDiceDisplay diceDisplay = go.AddComponent<ResultDiceDisplay>();
            diceDisplay.InitDiceDisplay(towerDie);
            diceDisplay.GetCameraFacingValue();

            resultDiceDisplay = diceDisplay;
        }

        resultDiceHolder.gameObject.SetActive(false);
    }
    public abstract void InitTowerData(Vector2Int positionOfCell, Die connectedDie);
    public abstract void RecieveBuffAfterRoll(Die die);
    protected void DisplayBuffAfterRoll(Die die)
    {
        DieFaceValue dieFaceValue = die.GetTopValue();

        resultDiceHolder.gameObject.SetActive(true);

        resultDiceDisplay.SetFaceData(dieFaceValue.Buff);
        towerDie.gameObject.SetActive(false);
    }
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
        towerDie.gameObject.SetActive(false);
        resultDiceHolder.gameObject.SetActive(false);

    }
}
