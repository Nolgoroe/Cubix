using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpell : SpellParent
{
    [SerializeField] private int damage;
    public override bool UseSpell(Die dieDragging)
    {
        //damage all current enemies

        List<EnemyParent> allEnemies = new List<EnemyParent>();
        allEnemies.AddRange(FindObjectsOfType<EnemyParent>());

        foreach (EnemyParent enemy in allEnemies)
        {
            enemy.RecieveDMG(damage);
        }

        return base.UseSpell(dieDragging);
    }

}
