using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSpell : SpellParent
{
    [SerializeField] private float speedMod;
    [SerializeField] private float timeToSlow;
    public override bool UseSpell(Die dieDragging)
    {
        //damage all current enemies

        List<EnemyParent> allEnemies = new List<EnemyParent>();
        allEnemies.AddRange(FindObjectsOfType<EnemyParent>());

        foreach (EnemyParent enemy in allEnemies)
        {
            enemy.BecomeSlowed(speedMod, timeToSlow);
        }

        return base.UseSpell(dieDragging);
    }
}
