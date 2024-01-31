using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Spells
{
    Damage, Slow, Freeze
}
public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    [SerializeField] private List<SpellParent> gameSpells;
    [SerializeField] private List<SpellParent> spellsInCooldown;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        foreach (SpellParent spellParent in gameSpells)
        {
            spellParent.gameObject.SetActive(false);

            if (Player.Instance.ReturnUnlockedSpells().Contains(spellParent.ReturnSpellType()))
            {
                spellParent.gameObject.SetActive(true);
            }
        }
    }
    public void AddSpellToList(SpellParent spell)
    {
        gameSpells.Add(spell);
    }
    public void AddSpellToCooldownList(SpellParent spell)
    {
        spellsInCooldown.Add(spell);
    }

    public void CountdownAllSpells()
    {
        foreach (SpellParent spell in spellsInCooldown)
        {
            spell.CountdownCooldown();            
        }

        for (int i = spellsInCooldown.Count - 1; i >= 0; i--)
        {
            if(spellsInCooldown[i].ReturnCurrentDieInSpell())
            {
                spellsInCooldown[i].EmptySpell();
            }

            if(spellsInCooldown[i].ReturnCanUseSpell())
            {
                spellsInCooldown.RemoveAt(i);
            }
        }

    }
}
