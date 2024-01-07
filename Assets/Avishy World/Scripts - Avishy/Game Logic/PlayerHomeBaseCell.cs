using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomeBaseCell : GridCell
{
    [SerializeField] int playerHealth;

    private void Start() //temp
    {
        playerHealth = 10; 
    }




















    public void RecieveDamage(EnemyParent enemy)
    {
        playerHealth -= enemy.ReturnEnemyDMG();

        if (playerHealth <= 0)
        {
            Debug.Log("You have lost!");
        }
    }

    public override void CopyDataFromToolCell(ToolGridCell toolGridCell)
    {
        base.CopyDataFromToolCell(toolGridCell);
    }

}
