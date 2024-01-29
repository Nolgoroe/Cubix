using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeTowerParentScript : TowerBaseParent
{
    [Header("Live data")]
    [SerializeField] List<GridCell> connectedPathCells;
    [SerializeField] GridCell RallyPoint;

    [Header("Troop Spawn Data")]
    [SerializeField] protected float spawnRate = 1;
    [SerializeField] protected float currentSpawnCooldown = 0;
    [SerializeField] int maxNumOfTroops;
    [SerializeField] int currentNumOfTroops;
    [SerializeField] List<TowerTroop> currentTowerTroops;

    [Header("Troop Combat Data")]
    [SerializeField] protected float troopDMG = 1;
    [SerializeField] protected float troopHP = 1;
    [SerializeField] protected float troopRange = 1;

    [Header("Preset Refs")]
    [SerializeField] protected GameObject troopPrefab;

    private float originalRange;
    private float originalSpawnRate;
    private float originalTroopHP;
    private float originalTroopRange;
    private float originalTroopDMG;

    protected override void OnEnable()
    {
        if (GameGridControls.Instance.rapidControls)
        {
            base.OnEnable();
        }

        currentNumOfTroops = 0;
        currentTowerTroops.Clear();
        connectedPathCells.Clear();
    }

    protected override void Start()
    {
        originalRange = range;
        originalSpawnRate = spawnRate;
        originalTroopHP = troopHP;
        originalTroopRange = troopRange;
        originalTroopDMG = troopDMG;

        base.Start();

        SetRangeIndicator();

        #region rotation to path
        //GridCell[,] gameGridCellsArray = GridManager.Instance.ReturnGridCellsArray();

        //int currentX = currentCellOnPos.x;
        //int currentY = currentCellOnPos.y;

        ////check up
        //if (currentY + 1 < GridManager.Instance.ReturnWidthHeight().y)
        //{
        //    if (gameGridCellsArray[currentX, currentY + 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
        //    {
        //        transform.LookAt(gameGridCellsArray[currentX, currentY + 1].transform);
        //        return;
        //    }
        //}

        ////check left
        //if (currentX - 1 > -1)
        //{
        //    if (gameGridCellsArray[currentX - 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
        //    {
        //        transform.LookAt(gameGridCellsArray[currentX - 1, currentY].transform);

        //        return;
        //    }
        //}

        ////check down
        //if (currentY - 1 > -1)
        //{
        //    if (gameGridCellsArray[currentX, currentY - 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
        //    {
        //        transform.LookAt(gameGridCellsArray[currentX, currentY - 1].transform);

        //        return;
        //    }
        //}

        ////check right
        //if (currentX + 1 < GridManager.Instance.ReturnWidthHeight().x)
        //{
        //    if (gameGridCellsArray[currentX + 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
        //    {
        //        transform.LookAt(gameGridCellsArray[currentX + 1, currentY].transform);

        //        return;
        //    }
        //}
        #endregion
    }
    protected override void Update()
    {
        base.Update();
        if (GameManager.gamePaused || isBeingDragged || isDisabled) return;

        if (currentNumOfTroops < maxNumOfTroops)
        {
            if (currentSpawnCooldown <= 0)
            {
                SpawnTroop();
                currentSpawnCooldown = spawnRate;
            }

            currentSpawnCooldown -= Time.deltaTime * GameManager.gameSpeed;
        }
    }

    private void SpawnTroop()
    {
        if (connectedPathCells.Count <= 0) return;

        float randomPosValueX = UnityEngine.Random.Range(-0.3f, 0.3f); //temp hardcoded
        float randomPosValueZ = UnityEngine.Random.Range(-0.3f, 0.3f); //temp hardcoded
        Vector3 randomPos = new Vector3(randomPosValueX, 0, randomPosValueZ);

        if (connectedPathCells.Count >= 0)
        {

            GameObject go = Instantiate(troopPrefab,
                troopPrefab.transform.position + RallyPoint.transform.position + randomPos, 
                Quaternion.identity);

            TowerTroop troop;
            go.TryGetComponent<TowerTroop>(out troop);

            if (troop)
            {
                troop.InitTroopData(this, troopHP, troopRange, troopDMG);
                currentTowerTroops.Add(troop);
            }
        }

        currentNumOfTroops++;
    }

    private void RefreshTroopsToRally()
    {
        foreach (TowerTroop troop in currentTowerTroops)
        {
            if(troop)
            {
                float randomPosValueX = UnityEngine.Random.Range(-0.3f, 0.3f); //temp hardcoded
                float randomPosValueZ = UnityEngine.Random.Range(-0.3f, 0.3f); //temp hardcoded
                Vector3 randomPos = new Vector3(randomPosValueX, 0, randomPosValueZ);

                troop.transform.position = troopPrefab.transform.position + RallyPoint.transform.position + randomPos;
            }
        }
    }






    public override void InitTowerData(Vector2Int positionOfCell, Die connectedDie)
    {
        connectedPathCells.Clear();

        currentCellOnPos = positionOfCell;

        currentSpawnCooldown = spawnRate;

        //check left, right up and down for path cells
        Vector2Int checkDown = new Vector2Int(currentCellOnPos.x, currentCellOnPos.y - 1);
        Vector2Int checkUp = new Vector2Int(currentCellOnPos.x, currentCellOnPos.y + 1);
        Vector2Int checkLeft = new Vector2Int(currentCellOnPos.x - 1, currentCellOnPos.y);
        Vector2Int checkRight = new Vector2Int(currentCellOnPos.x + 1, currentCellOnPos.y);

        Vector2Int[] directions = new Vector2Int[] { checkDown, checkUp, checkLeft, checkRight };

        foreach (Vector2Int pos in directions)
        {
            if(pos.x > -1 && pos.x < GridManager.Instance.ReturnWidthHeight().x
                && pos.y >-1 && pos.y < GridManager.Instance.ReturnWidthHeight().y)
            {
                GridCell cell = GridManager.Instance.ReturnCellAtVector(pos);
                if (cell.ReturnTypeOfCell() == TypeOfCell.enemyPath)
                {
                    connectedPathCells.Add(cell);
                }
            }
        }

        int randomIndex = 0;
        randomIndex = UnityEngine.Random.Range(0, connectedPathCells.Count);
        RallyPoint = connectedPathCells[randomIndex];

        towerDie = connectedDie;
        towerDie.transform.SetParent(resultDiceHolder);

        specialAttackUnlocked = towerDie.ReturnSpecialAttackUnlcoked();
    }

    public void LoseTroop(TowerTroop lostTroop)
    {
        currentNumOfTroops--;
    }

    public void CleanTroopsAtWaveStart()
    {
        for (int i = currentTowerTroops.Count - 1; i >= 0; i--)
        {
            if (currentTowerTroops[i] == null)
            {
                currentTowerTroops.RemoveAt(i);
            }

        }
    }
    protected override void CleanTroopsCompletely()
    {
        for (int i = currentTowerTroops.Count - 1; i >= 0; i--)
        {
            if(currentTowerTroops[i] != null)
            {
                Destroy(currentTowerTroops[i].gameObject);
            }
        }
        currentNumOfTroops = 0;
        currentTowerTroops.Clear();
    }
    public override void RecieveBuffAfterRoll(Die die)
    {
        DieFaceValue dieFaceValue = towerDie.GetTopValue();

        switch (dieFaceValue.Buff.Type)
        {
            case BuffType.None:
                break;
            case BuffType.Dmg:
                troopDMG += originalTroopDMG * (dieFaceValue.Buff.Value / 100);
                break;
            case BuffType.Range:
                troopRange += originalTroopRange * (dieFaceValue.Buff.Value / 100);
                range += originalRange * (dieFaceValue.Buff.Value / 100);

                SetRangeIndicator();
                break;
            case BuffType.HP:
                troopHP += originalTroopHP * (dieFaceValue.Buff.Value / 100);
                break;
            case BuffType.time:
                spawnRate -= originalSpawnRate * (dieFaceValue.Buff.Value / 100);
                break;
            default:
                break;
        }

        AddNewTowerBuff(dieFaceValue, die);
    }
    public override void RecieveRandomBuff(Die die)
    {
        int randomBuffIndex = UnityEngine.Random.Range(0, die.GetAllFaces().Length);
        DieFaceValue dieFaceValue = die.GetAllFaces()[randomBuffIndex].GetFaceValue();

        switch (dieFaceValue.Buff.Type)
        {
            case BuffType.None:
                //if we get none, just return Damage for now - Temp
                troopDMG += originalTroopDMG * (dieFaceValue.Buff.Value / 100);
                dieFaceValue = die.GetAllFaces()[2].GetFaceValue(); //temp
                break;
            case BuffType.Dmg:
                troopDMG += originalTroopDMG * (dieFaceValue.Buff.Value / 100);
                break;
            case BuffType.Range:
                troopRange += originalTroopRange * (dieFaceValue.Buff.Value / 100);
                range += originalRange * (dieFaceValue.Buff.Value / 100);

                SetRangeIndicator();
                break;
            case BuffType.HP:
                troopHP += originalTroopHP * (dieFaceValue.Buff.Value / 100);
                break;
            case BuffType.time:
                spawnRate -= originalSpawnRate * (dieFaceValue.Buff.Value / 100);
                break;
            default:
                break;
        }

        AddNewTowerBuff(dieFaceValue, die);
    }

    public override void OnHoverOverOccupyingCell(bool isHover)
    {
        if (rangeIndicator)
            rangeIndicator.gameObject.SetActive(isHover ? true : false);

        foreach (TowerTroop troop in currentTowerTroops)
        {
            troop.OnHoverOverParentTower(isHover);
        }
    }

    public void SetRallyPoint(GridCell cell)
    {
        if (cell.ReturnTypeOfCell() != TypeOfCell.enemyPath) return;

        if (Vector3.Distance(transform.position, cell.transform.position) < range)
        {
            Debug.Log(Vector3.Distance(transform.position, cell.transform.position));

            if(RallyPoint != cell)
            {
                RallyPoint = cell;

                RefreshTroopsToRally();
            }
        }
    }

    public override List<string> DisplayTowerStats()
    {
        List<string> stringList = new List<string>()
        { "Range: " + range.ToString(),
          "Required Color: " + requiredCellColorType.ToString(),
          "Special Unlcoked: " + (specialAttackUnlocked ? "True" : "False"),
          "Spawn Rate: " + spawnRate.ToString(),
          "Max Troops: " + maxNumOfTroops.ToString(),
          "Troop Damage: " + troopDMG.ToString(), 
          "Troop Range: " + troopRange.ToString(),
          "Troop Prefab: " + troopPrefab.gameObject.name
        };

        return stringList;
    }
}
