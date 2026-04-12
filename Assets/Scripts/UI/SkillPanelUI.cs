using UnityEngine;

public class SkillPanelUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private SkillIconUI prefab;
    
    private SkillIconUI[] _activeIcons;

    public void UpdateSkills(CharacterClassData data)
    {
        Clear();

        var skills = data.startingSkills;

        _activeIcons = new SkillIconUI[skills.Length];
        
        for (var i = 0; i < skills.Length; i++)
        {
            var icon = Instantiate(prefab, container);
            icon.Initialize(skills[i]);
            
            _activeIcons[i] = icon;
        }
    }

    private void Clear()
    {
        for (var i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }
}