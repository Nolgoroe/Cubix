using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell Creation", menuName = "ScriptableObjects/Spell")]
public class SpellSO : ItemShopParentSO
{
    [Header("To Buy")]
    public Spells spellToRecieve;


    public void AddSpellToPlayer(SpellSO spellSO)
    {
        Player.Instance.AddToUnlockedSpells(spellSO.spellToRecieve);
    }


    public bool CheckHasSpell(out string systemMessage)
    {
        systemMessage = "";

        if (Player.Instance.ReturnUnlockedSpells().Contains(spellToRecieve))
        {
            systemMessage = "Already has spell";

            return true;
        }
        else
        {
            return false;
        }
    }
}
