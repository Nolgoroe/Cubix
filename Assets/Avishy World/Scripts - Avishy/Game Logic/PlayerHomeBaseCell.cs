using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomeBaseCell : GridCell
{
    [SerializeField] int playerHealth;
    [SerializeField] int currentPlayerHealth;
    [SerializeField] Transform dangerIcon;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] float detectionRange;

    protected override void Start() //temp
    {
        base.Start();
        playerHealth = 1000;
        currentPlayerHealth = playerHealth;

        dangerIcon = transform.GetChild(0); // temp
        enemyLayerMask |= (1 << LayerMask.NameToLayer("Flying Enemy")); // temp
        enemyLayerMask |= (1 << LayerMask.NameToLayer("Enemy")); // temp
        detectionRange = 5; // temp

        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, playerHealth);
    }

    private void Update()
    {
        CheckBaseRadius();
    }

    private bool CheckBaseRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayerMask);

        if(hitColliders.Length > 0)
        {
            dangerIcon.gameObject.SetActive(true);

            return true;
        }
        dangerIcon.gameObject.SetActive(false);

        return false;
    }















    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }



    public void RecieveDamage(EnemyParent enemy)
    {
        currentPlayerHealth -= 1;

        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
        }

        UIManager.Instance.UpdatePlayerHealth(currentPlayerHealth, playerHealth);

        if (currentPlayerHealth <= 0)
        {
            Debug.Log("You have lost!");

            UIManager.Instance.DisplayEndGameScreen(false);
            GameManager.isDead = true;
            return;
        }

    }

    public override void CopyDataFromToolCell(ToolGridCell toolGridCell)
    {
        base.CopyDataFromToolCell(toolGridCell);
    }

}
