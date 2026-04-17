using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoadSaveUI : MonoBehaviour
{
    public static bool ReplaceMode;

    [Header("UI")]
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private SaveSlotUI slotPrefab;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Button backButton;
    
    private readonly List<SaveSlotUI> _slots = new();
    
    private const int MaxSlots = 3;

    private void Start()
    {
        SetupHeader();
        GenerateSlots();
        SetupBackButton();
    }

    private void SetupHeader()
    {
        headerText.text = ReplaceMode ? "Select Slot to Replace" : "Load Game";
        
        headerText.color = ReplaceMode ? Color.red : Color.white;
    }

    private void GenerateSlots()
    {
        foreach (Transform child in slotsContainer)
            Destroy(child.gameObject);
        
        _slots.Clear();

        for (var i = 0; i < MaxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, slotsContainer);

            var data = SaveSystem.Load(i);
            
            slot.Initialize(i, data, OnSlotClicked);
            
            _slots.Add(slot);
        }
    }

    private void OnSlotClicked(int slotIndex, RunSaveData data)
    {
        if (ReplaceMode)
        {
            SaveSystem.Delete(slotIndex);
            
            GameSession.Instance.StartNewRun(slotIndex);
            
            ReplaceMode = false;
            
            SceneLoader.Instance.LoadScene(SceneId.CharacterCreationPlayer);
            return;
        }

        if (data == null)
            return;
        
        GameSession.Instance.LoadFromSave(data, slotIndex);
        
        SceneLoader.Instance.LoadScene(SceneId.GameScene);
    }

    private void SetupBackButton()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            ReplaceMode = false;
            SceneLoader.Instance.LoadScene(SceneId.MainMenu);
        });
    }
}