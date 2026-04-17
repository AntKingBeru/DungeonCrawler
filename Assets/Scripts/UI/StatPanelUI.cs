using System.Globalization;
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
        hp.text = data.maxHealth.ToString(CultureInfo.InvariantCulture);
        mana.text = data.maxMana.ToString(CultureInfo.InvariantCulture);
        damage.text = data.damage.ToString(CultureInfo.InvariantCulture);
        speed.text = data.movementSpeed.ToString(CultureInfo.InvariantCulture);
    }
}