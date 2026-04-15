using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyUIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Slider playerHealth;
    [SerializeField] private Slider playerMana;
    [SerializeField] private Image playerIcon;
    [SerializeField] private TMP_Text playerName;
    
    [Header("Party")]
    [SerializeField] private PartyMemberUI[] partyMembers;
    
    private CharacterSelectionData _player;
    private CharacterSelectionData[] _party;

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnHealthChanged -= OnPlayerHealthChanged;
            _player.OnManaChanged -= OnPlayerManaChanged;
        }
    }

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _player = GameSession.Instance.Player;
        _party = GameSession.Instance.Party;

        SetupPlayer();
        SetupParty();
    }

    private void SetupPlayer()
    {
        playerIcon.sprite = _player.@class.icon;
        playerName.text = _player.name;

        playerHealth.maxValue = _player.@class.maxHealth;
        playerMana.maxValue = _player.@class.maxMana;

        _player.OnHealthChanged += OnPlayerHealthChanged;
        _player.OnManaChanged += OnPlayerManaChanged;

        OnPlayerHealthChanged(_player.currentHealth);
        OnPlayerManaChanged(_player.currentMana);
    }
    
    private void OnPlayerHealthChanged(float value)
    {
        playerHealth.value = value;
    }
    
    private void OnPlayerManaChanged(float value)
    {
        playerMana.value = value;
    }

    private void SetupParty()
    {
        for (var i = 0; i < partyMembers.Length; i++)
            partyMembers[i].Initialize(_party[i]);
    }

    private void Update()
    {
        UpdatePlayer();
        UpdateParty();
    }

    private void UpdatePlayer()
    {
        playerHealth.value = _player.currentHealth;
        playerMana.value = _player.currentMana;
    }

    private void UpdateParty()
    {
        foreach (var member in partyMembers)
            member.UpdateUI();
    }
}