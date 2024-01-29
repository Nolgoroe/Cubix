using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellParent : MonoBehaviour
{
    [SerializeField] GameObject diceHolder;
    [SerializeField] Die currentDieInSpell;
    [SerializeField] int cooldown;
    [SerializeField] int currentCooldown;
    [SerializeField] CellTypeColor requiredColor;

    private void Start()
    {
        //SpellManager.Instance.AddSpellToList(this);
    }

    public bool SnapToHolder(Die die)
    {
        if(die.ReturnDieColorType() == requiredColor || currentCooldown > 0)
        {
            die.transform.position = diceHolder.transform.position;
            return true;
        }

        return false;
    }

    public virtual bool UseSpell(Die dieDragging)
    {
        if (currentCooldown > 0) return false;
        //activate spell here.
        currentDieInSpell = dieDragging;

        SpellManager.Instance.AddSpellToCooldownList(this);
        currentCooldown = cooldown;

        return true;
    }

    public void CountdownCooldown()
    {
        currentCooldown--;
    }

    public bool ReturnCanUseSpell()
    {
        return currentCooldown <= 0;
    }

    public Die ReturnCurrentDieInSpell()
    {
        return currentDieInSpell;
    }

    public void EmptySpell()
    {
        DiceManager.Instance.AddDiceToResources(currentDieInSpell);
        currentDieInSpell.BackToPlayerArea();
        currentDieInSpell = null;
    }
}
