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
    
    private void OnDestroy()
    {
        if (_data != null)
        {
            _data.OnHealthChanged -= OnHealthChanged;
            _data.OnManaChanged -= OnManaChanged;
        }
    }

    public void Initialize(CharacterSelectionData data)
    {
        _data = data;

        icon.sprite = data.@class.icon;
        nameText.text = data.@class.className;

        health.maxValue = data.MaxHealth;
        mana.maxValue = data.MaxMana;

        _data.OnHealthChanged += OnHealthChanged;
        _data.OnManaChanged += OnManaChanged;
        
        OnHealthChanged(_data.currentHealth);
        OnManaChanged(_data.currentMana);
    }
    
    private void OnHealthChanged(float value)
    {
        health.value = value;
    }
    
    private void OnManaChanged(float value)
    {
        mana.value = value;
    }

    public void UpdateUI()
    {
        health.value = _data.currentHealth;
        mana.value = _data.currentMana;
    }
}