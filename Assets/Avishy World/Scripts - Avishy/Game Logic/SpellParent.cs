using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellParent : MonoBehaviour
{
    [SerializeField] GameObject diceHolder;
    [SerializeField] Die currentDieInSpell;
    [SerializeField] int cooldown;
    [SerializeField] int currentCooldown;
    [SerializeField] CellTypeColor requiredColor;
    //[SerializeField] TMP_Text currentContdownText;
    [SerializeField] Spells spellType;
    [SerializeField] Image cooldownImage;

    private void Awake()
    {
        SpellManager.Instance.AddSpellToList(this);

        cooldownImage.fillAmount = 0;
    }
    public bool SnapToHolder(Die die)
    {
        if(die.ReturnDieColorType() == requiredColor && ReturnCanUseSpell())
        {
            die.transform.position = diceHolder.transform.position;
            die.transform.rotation = diceHolder.transform.rotation;
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

        cooldownImage.fillAmount =  1;
        return true;
    }

    public void CountdownCooldown()
    {
        currentCooldown--;

        LeanTween.value(cooldownImage.gameObject, cooldownImage.fillAmount, (float)currentCooldown / cooldown, 0.5f).setOnUpdate((float val) =>
        {
            cooldownImage.fillAmount = val;
        });
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
