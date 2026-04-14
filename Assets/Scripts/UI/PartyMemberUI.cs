using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private Slider health;
    [SerializeField] private Slider mana;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    
    private CharacterSelectionData _data;

    public void Initialize(CharacterSelectionData data)
    {
        _data = data;

        icon.sprite = data.@class.icon;
        nameText.text = data.@class.className;

        health.maxValue = data.@class.maxHealth;
        mana.maxValue = data.@class.maxMana;
    }

    public void UpdateUI()
    {
        health.value = _data.currentHealth;
        mana.value = _data.currentMana;
    }
}