using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;

    private SkillData _data;

    public void Initialize(SkillData skill)
    {
        _data = skill;
        icon.sprite = skill.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}