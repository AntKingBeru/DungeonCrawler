using UnityEngine;
using TMPro;

public class ClassUIBinder : MonoBehaviour
{
    [SerializeField] private TMP_Text description;

    public void UpdateUI(CharacterClassData data)
    {
        description.text = data.description;
    }
}