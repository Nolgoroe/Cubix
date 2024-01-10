using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    protected virtual void Update()
    {
        if(currentNumOfTroops < maxNumOfTroops)
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
        Vector3 randomPos = new Vector3(randomPosValueX, 0.25f, randomPosValueZ);

        if (connectedPathCells.Count >= 0)
        {
            randomIndex = UnityEngine.Random.Range(0, connectedPathCells.Count);

            GameObject go = Instantiate(troopPrefab, connectedPathCells[randomIndex].transform.position + randomPos, Quaternion.identity);

            TowerTroop troop;
            go.TryGetComponent<TowerTroop>(out troop);

            if (troop)
            {
                troop.InitTroopData(this);
            }
        }

        currentNumOfTroops++;
    }









    public override void InitTowerData(Vector2Int positionOfCell)
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
    }

    public void LoseTroop()
    {
        currentNumOfTroops--;

        if(currentNumOfTroops < 0)
        {
            currentNumOfTroops = 0;
        }
    }

}
