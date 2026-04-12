using System.Linq;
using UnityEngine;

public class PartyCreationController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharacterClassData[] classes;
    
    [Header("UI")]
    [SerializeField] private PartySlotUI[] slots;
    [SerializeField] private ClassButtonUI classButtonPrefab;
    
    [Header("Formations")]
    [SerializeField] private FormationData[] formations;
    [SerializeField] private Transform formationParent;
    [SerializeField] private FormationButtonUI formationButtonPrefab;
    
    private const int MaxColors = 4;
    
    private PartyMemberData[] _members;
    private FormationButtonUI[] _formationButtons;
    private int _currentFormationIndex;
    
    private void Start()
    {
        InitializeData();
        GenerateAllClassButtons();
        InitializeDefaults();
        InitializeColorButtons();
        GenerateFormationButtons();
        SelectFormation(0);
        RefreshAll();
    }

    private void InitializeData()
    {
        _members = new PartyMemberData[slots.Length];

        for (var i = 0; i < _members.Length; i++)
        {
            _members[i] = new PartyMemberData();
        }
    }

    private void InitializeDefaults()
    {
        for (var i = 0; i < _members.Length; i++)
        {
            _members[i].@class = classes[i % classes.Length];
            _members[i].colorIndex = GetFirstValidColor(i, _members[i].@class);
        }
    }
    
    private void GenerateAllClassButtons()
    {
        for (var slotIndex = 0; slotIndex < slots.Length; slotIndex++)
        {
            GenerateClassButtonsForSlot(slotIndex);
        }
    }
    
    private void InitializeColorButtons()
    {
        for (var slotIndex = 0; slotIndex < slots.Length; slotIndex++)
        {
            var buttons = slots[slotIndex].ColorButtons;

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Initialize(
                    i,
                    slotIndex,
                    this,
                    classes[0].colorVariants[i].color
                );
            }
        }
    }
    
    private void GenerateFormationButtons()
    {
        _formationButtons = new FormationButtonUI[formations.Length];

        for (var i = 0; i < formations.Length; i++)
        {
            var btn = Instantiate(formationButtonPrefab, formationParent);

            btn.Initialize(i, formations[i], OnFormationSelected);

            _formationButtons[i] = btn;
        }
    }
    
    private void OnFormationSelected(int index)
    {
        SelectFormation(index);
    }
    
    private void SelectFormation(int index)
    {
        _currentFormationIndex = index;

        GameSession.Instance.SetFormation(formations[_currentFormationIndex]);

        for (var i = 0; i < _formationButtons.Length; i++)
        {
            _formationButtons[i].SetSelected(i == _currentFormationIndex);
        }
    }

    private void GenerateClassButtonsForSlot(int slotIndex)
    {
        var parent = slots[slotIndex].ClassButtonParent;

        for (var i = 0; i < classes.Length; i++)
        {
            var capturedSlot = slotIndex;

            var btn = Instantiate(classButtonPrefab, parent);

            btn.Initialize(i, classes[i].icon, (index) =>
            {
                OnClassSelected(capturedSlot, index);
            });
        }
    }

    private void OnClassSelected(int slot, int classIndex)
    {
        _members[slot].@class = classes[classIndex];
        
        if (!IsValid(slot, _members[slot].@class, _members[slot].colorIndex))
        {
            _members[slot].colorIndex = GetFirstValidColor(slot, _members[slot].@class);
        }

        RefreshAll();
    }
    
    public void OnColorSelected(int slot, int colorIndex)
    {
        if (!IsValid(slot, _members[slot].@class, colorIndex))
            return;

        _members[slot].colorIndex = colorIndex;

        RefreshAll();
    }
    
    private bool IsValid(int slot, CharacterClassData c, int color)
    {
        var player = GameSession.Instance.Player;
        
        if (player.@class == c && player.colorIndex == color)
            return false;
        
        return _members.Where((_, i) => i != slot)
            .All(member => member.@class != c || member.colorIndex != color);
    }

    private int GetFirstValidColor(int slot, CharacterClassData c)
    {
        for (var i = 0; i < MaxColors; i++)
        {
            if (IsValid(slot, c, i))
                return i;
        }

        return 0;
    }
    
    private void RefreshAll()
    {
        for (var i = 0; i < slots.Length; i++)
        {
            RefreshSlot(i);
        }
    }

    private void RefreshSlot(int i)
    {
        var member = _members[i];
        var slot = slots[i];

        slot.Preview.SetClass(member.@class);
        slot.Preview.SetColor(member.colorIndex);

        UpdateColorButtons(i);
        PushToGameSession(i);
    }

    private void UpdateColorButtons(int slotIndex)
    {
        var slot = slots[slotIndex];
        var buttons = slot.ColorButtons;

        for (var i = 0; i < buttons.Length; i++)
        {
            var valid = IsValid(slotIndex, _members[slotIndex].@class, i);

            buttons[i].SetInteractable(valid);
            buttons[i].SetSelected(i == _members[slotIndex].colorIndex);
        }
    }

    private void PushToGameSession(int slot)
    {
        var member = _members[slot];

        var data = new CharacterSelectionData
        {
            @class = member.@class,
            colorIndex = member.colorIndex,
            name = "" // party members have no names
        };

        GameSession.Instance.SetPartyMember(slot, data);
    }

    public void Confirm()
    {
        SceneLoader.Instance.LoadScene(SceneId.GameScene);
    }

    public void OnBackPressed()
    {
        SceneLoader.Instance.LoadScene(SceneId.CharacterCreationPlayer);   
    }
}