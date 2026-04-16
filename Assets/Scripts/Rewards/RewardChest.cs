using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class RewardChest : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform lid;
    [SerializeField] private float openAngle = 110f;
    [SerializeField] private float openSpeed = 5f;
    
    [Header("Interaction")]
    [SerializeField] private InputActionReference openAction;
    [SerializeField] private string playerTag = "Player";

    private RewardData _reward;
    private bool _isOpened;
    private bool _playerInside;

    public void Initialize(RewardData reward)
    {
        _reward = reward;
    }

    private void OnEnable()
    {
        openAction.action.Enable();
    }
    
    private void OnDisable()
    {
        openAction.action.Disable();
    }

    private void Update()
    {
        if (_playerInside && !_isOpened && openAction.action.triggered)
            Open();
    }

    private void Open()
    {
        _isOpened = true;

        ApplyReward();

        StartCoroutine(OpenLidRoutine());
    }
    
    private IEnumerator OpenLidRoutine()
    {
        var startRot = lid.localRotation;
        var endRot = Quaternion.Euler(openAngle, 0f, 0f);

        var t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            lid.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
        
        yield return new WaitForSeconds(2f);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            lid.localRotation = Quaternion.Lerp(endRot, startRot, t);
            yield return null;
        }
    }

    private void ApplyReward()
    {
        var player = GameSession.Instance.Player;
        var party = GameSession.Instance.Party;

        switch (_reward.type)
        {
            case RewardType.UnlockSkill:
                UnlockSkill(player, _reward.skillIndex);
                foreach (var member in party)
                    UnlockSkill(member, _reward.skillIndex);
                break;
            case RewardType.StatBoost:
                ApplyStatBoost(player, _reward);
                foreach (var member in party)
                    ApplyStatBoost(member, _reward);
                break;
        }
    }

    private void UnlockSkill(CharacterSelectionData character, int index)
    {
        var unlocks = character.@class.unlockableSkills;

        if (index < 0 || index >= unlocks.Length)
            return;
        
        var skill = unlocks[index];
        
        if (!character.unlockedSkills.Contains(skill))
            character.unlockedSkills.Add(skill);
    }

    private void ApplyStatBoost(CharacterSelectionData character, RewardData reward)
    {
        character.@class.maxHealth += reward.healthBonus;
        character.@class.maxMana += reward.manaBonus;
        character.@class.damage += reward.damageBonus;
        
        character.SetHealth(character.@class.maxHealth);
        character.SetMana(character.@class.maxMana);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            _playerInside = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            _playerInside = false;
    }
}