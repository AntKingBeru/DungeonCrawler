using UnityEngine;
using TMPro;

public class StatPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text speed;

    public void UpdateStat(CharacterClassData data)
    {
        hp.text = data.maxHealth.ToString();
        mana.text = data.maxMana.ToString();
        damage.text = data.damage.ToString();
        speed.text = data.movementSpeed.ToString();
    }
}