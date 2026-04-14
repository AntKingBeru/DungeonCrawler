using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] slots;

    public void Initialize(SkillRuntime[] skills)
    {
        for (var i = 0; i < slots.Length; i++)
        {
            if (i >= skills.Length)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }
            
            slots[i].gameObject.SetActive(true);
            slots[i].Initialize(skills[i]);
        }
    }

    public void UpdateCooldowns(SkillRuntime[] skills)
    {
        for (var i = 0; i < skills.Length; i++)
            slots[i].UpdateVisual();
    }
    
    public void SetAvailable(int index, bool available)
    {
        if (index >= slots.Length)
            return;

        slots[index].SetAvailable(available);
    }

    public RectTransform GetSlot(int index)
    {
        return slots[index].GetComponent<RectTransform>();
    }
}