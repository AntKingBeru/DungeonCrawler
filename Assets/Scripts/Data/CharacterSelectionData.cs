using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSelectionData
{
    public CharacterClassData @class;
    public int colorIndex;
    public string name;
    
    public float currentHealth;
    public float currentMana;
    
    public float bonusMaxHealth;
    public float bonusMaxMana;
    public float bonusDamage;
    
    public float MaxHealth => @class.maxHealth + bonusMaxHealth;
    public float MaxMana => @class.maxMana + bonusMaxMana;
    public float Damage => @class.damage + bonusDamage;

    public List<SkillData> unlockedSkills = new();
    
    public event Action<float> OnHealthChanged;
    public event Action<float> OnManaChanged;

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void SetMana(float value)
    {
        currentMana = Mathf.Clamp(value, 0, MaxMana);
        OnManaChanged?.Invoke(currentMana);
    }
}