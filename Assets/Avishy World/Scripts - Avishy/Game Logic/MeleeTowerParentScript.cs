using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeTowerParentScript : TowerBaseParent
{
    [Header("Live data")]
    [SerializeField] List<GridCell> connectedPathCells;

    [Header("Troop Spawn Data")]
    [SerializeField] protected GameObject troopPrefab;
    [SerializeField] int maxNumOfTroops;
    [SerializeField] protected float spawnRate = 1;
    [SerializeField] protected float currentSpawnCooldown = 0;
    [SerializeField] int currentNumOfTroops;
    [SerializeField] List<TowerTroop> currentTowerTroops;

    protected override void Start()
    {
        base.Start();
        GridCell[,] gameGridCellsArray = GridManager.Instance.ReturnGridCellsArray();

        int currentX = currentCellOnPos.x;
        int currentY = currentCellOnPos.y;

        //check up
        if (currentY + 1 < GridManager.Instance.ReturnWidthHeight().y)
        {
            if (gameGridCellsArray[currentX, currentY + 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX, currentY + 1].transform);
                return;
            }
        }

        //check left
        if (currentX - 1 > -1)
        {
            if (gameGridCellsArray[currentX - 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX - 1, currentY].transform);

                return;
            }
        }

        //check down
        if (currentY - 1 > -1)
        {
            if (gameGridCellsArray[currentX, currentY - 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX, currentY - 1].transform);

                return;
            }
        }

        //check right
        if (currentX + 1 < GridManager.Instance.ReturnWidthHeight().x)
        {
            if (gameGridCellsArray[currentX + 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX + 1, currentY].transform);

                return;
            }
        }
    }
    protected virtual void Update()
    {
        if (GameManager.gameSpeed == 0) return;

        if (currentNumOfTroops < maxNumOfTroops)
        {
            if (currentSpawnCooldown <= 0)
            {
                SpawnTroop();
                currentSpawnCooldown = (1 / spawnRate) / GameManager.gameSpeed;
            }

            currentSpawnCooldown -= Time.deltaTime;
        }
    }

    private void SpawnTroop()
    {
        if (connectedPathCells.Count <= 0) return;

        int randomIndex = 0;
        float randomPosValueX = UnityEngine.Random.Range(-0.3f, 0.3f); //temp hardcoded
        float randomPosValueZ = UnityEngine.Random.Range(-0.3f, 0.3f); //temp hardcoded
        Vector3 randomPos = new Vector3(randomPosValueX, 0, randomPosValueZ);

        if (connectedPathCells.Count >= 0)
        {
            randomIndex = UnityEngine.Random.Range(0, connectedPathCells.Count);

            GameObject go = Instantiate(troopPrefab,
                troopPrefab.transform.position + connectedPathCells[randomIndex].transform.position + randomPos, 
                Quaternion.identity);

            TowerTroop troop;
            go.TryGetComponent<TowerTroop>(out troop);

            if (troop)
            {
                troop.InitTroopData(this);
                currentTowerTroops.Add(troop);
            }
        }

        currentNumOfTroops++;
    }









    public override void InitTowerData(Vector2Int positionOfCell, Die connectedDie)
    {
        currentCellOnPos = positionOfCell;

        currentSpawnCooldown = (1 / spawnRate) / GameManager.gameSpeed;

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
        

        towerDie = connectedDie;

        SpawnBuffCubeOnCreation();
    }

    public void LoseTroop(TowerTroop lostTroop)
    {
        currentNumOfTroops--;

        if(currentNumOfTroops < 0)
        {
            currentNumOfTroops = 0;
        }

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
    public override void RecieveBuffAfterRoll(Die die)
    {
        DieFaceValue dieFaceValue = towerDie.GetTopValue();

        switch (dieFaceValue.Buff.Type)
        {
            case BuffType.Speed:
                break;
            case BuffType.Damage:
                break;
            case BuffType.Fire:
                break;
            case BuffType.AttackSpeed:
                //add attack speed to relavent tower
                spawnRate += 0.1f;
                break;
            default:
                break;
        }

        AddNewTowerBuff(dieFaceValue, die);
    }

    public override void OnHoverOverOccupyingCell(bool isHover)
    {
        foreach (TowerTroop troop in currentTowerTroops)
        {
            troop.OnHoverOverParentTower(isHover);
        }
    }
}
