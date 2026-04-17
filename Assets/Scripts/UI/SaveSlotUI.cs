using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject highlight;

    private int _slotIndex;

    public void Initialize(int slot, RunSaveData data, Action<int, RunSaveData> onClick)
    {
        _slotIndex = slot;

        var hasData = data != null;

        titleText.text = $"Slot {slot + 1}";
        
        if (!hasData)
        {
            infoText.text = "No Save Data";
        }
        else
        {
            infoText.text =
                $"{data.player}\n" +
                $"Bosses: {data.bossesDefeated}\n" +
                $"{data.timestamp}";
        }
        
        button.interactable = hasData || LoadSaveUI.ReplaceMode;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(_slotIndex, data));
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
    }
}