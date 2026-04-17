using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveData
{
    public string classId;

    public float currentHealth;
    public float currentMana;
    
    public float bonusMaxHealth;
    public float bonusMaxMana;
    public float bonusDamage;
    
    public List<string> unlockedSkills = new ();
}