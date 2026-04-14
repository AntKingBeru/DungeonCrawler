using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private CanvasGroup canvasGroup;

    private SkillRuntime _skill;

    public void Initialize(SkillRuntime skill)
    {
        _skill = skill;
        
        icon.sprite = skill.Data.icon;

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (_skill == null)
            return;

        cooldownOverlay.fillAmount = Mathf.Lerp(
            cooldownOverlay.fillAmount,
            _skill.CooldownNormalized,
            Time.deltaTime * 15f
        );
        
        if (cooldownText)
        {
            if (_skill.IsReady)
            {
                cooldownText.gameObject.SetActive(false);
            }
            else
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.CeilToInt(_skill.CooldownRemaining).ToString();
            }
        }
    }

    public void SetAvailable(bool available)
    {
        if (canvasGroup)
            canvasGroup.alpha = available ? 1 : 0.4f;
    }
    
    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}