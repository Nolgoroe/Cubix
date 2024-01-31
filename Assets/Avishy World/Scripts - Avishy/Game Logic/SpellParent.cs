using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellParent : MonoBehaviour
{
    [SerializeField] GameObject diceHolder;
    [SerializeField] Die currentDieInSpell;
    [SerializeField] int cooldown;
    [SerializeField] int currentCooldown;
    [SerializeField] CellTypeColor requiredColor;
    [SerializeField] TMP_Text currentContdownText;
    [SerializeField] Spells spellType;

    private void Awake()
    {
        SpellManager.Instance.AddSpellToList(this);

        currentContdownText.text = "Usable!";
    }
    public bool SnapToHolder(Die die)
    {
        if(die.ReturnDieColorType() == requiredColor && ReturnCanUseSpell())
        {
            die.transform.position = diceHolder.transform.position;
            return true;
        }

        return false;
    }

    public virtual bool UseSpell(Die dieDragging)
    {
        if (!ReturnCanUseSpell()) return false;

        //activate spell here.
        currentDieInSpell = dieDragging;

        SpellManager.Instance.AddSpellToCooldownList(this);
        currentCooldown = cooldown;

        currentContdownText.text =  "Cooldown: " + currentCooldown.ToString();
        return true;
    }

    public void CountdownCooldown()
    {
        currentCooldown--;

        if(currentCooldown <= 0)
        {
            currentContdownText.text = "Usable!";
        }
        else
        {
            currentContdownText.text = "Cooldown: " + currentCooldown.ToString();
        }
    }

    public bool ReturnCanUseSpell()
    {
        return currentCooldown <= 0 && currentDieInSpell == null;
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

    public Spells ReturnSpellType()
    {
        return spellType;
    }
}
