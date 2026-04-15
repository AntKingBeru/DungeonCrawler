using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SkillController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference[] skillActions;
    [SerializeField] private float holdThreshold = 0.5f;
    
    [Header("Setup")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SkillExecuter skillExecuter;
    
    [Header("Mana Regen")]
    [SerializeField] private float manaRegenPerSecond = 5f;
    [SerializeField] private float manaRegenDelay = 1.5f;
    
    [Header("UI")]
    private HotbarUI _hotbarUI;
    
    [Header("Runtime")]
    private SkillRuntime[] _skills;
    private CharacterSelectionData _data;
    
    [Header("Animation")]
    private Animator _animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private float[] _holdTimers;
    private int _currentTooltipIndex = -1;
    private float _regenTimer;
    
    public UnityEvent<SkillRuntime> onSkillUsed;

    public void Initialize(CharacterSelectionData data, HotbarUI hotbar, Animator animator = null)
    {
        _data = data;
        _hotbarUI = hotbar;
        _animator = animator;
        
        BuildSkills();

        _hotbarUI.Initialize(_skills);
        
        _holdTimers = new float[_skills.Length];
    }

    private void BuildSkills()
    {
        var allSkills = new List<SkillData>(_data.@class.startingSkills);
        
        var active = new List<SkillData>();
        var passive = new List<SkillData>();

        foreach (var skill in allSkills)
        {
            if (skill.type == SkillType.Active)
                active.Add(skill);
            else
                passive.Add(skill);
        }
        
        var sorted = new List<SkillData>();
        sorted.AddRange(active);
        sorted.AddRange(passive);
        
        _skills = new SkillRuntime[sorted.Count];

        for (var i = 0; i < sorted.Count; i++)
        {
            _skills[i] = new SkillRuntime
            {
                Data = sorted[i],
            };
        }
    }

    private void OnEnable()
    {
        foreach (var skillAction in skillActions)
            skillAction.action.Enable();
    }
    
    private void OnDisable()
    {
        foreach (var skillAction in skillActions)
            skillAction.action.Disable();
    }

    private void Update()
    {
        TickCooldowns();
        HandleInput();
        HandleManaRegen();
        UpdateUI();
    }
    
    private void TickCooldowns()
    {
        foreach (var skill in _skills)
            skill.Tick(Time.deltaTime);
    }

    private void HandleInput()
    {
        for (var i = 0; i < _skills.Length; i++)
        {
            if (i >= skillActions.Length)
                continue;
            
            var action = skillActions[i].action;

            if (action.IsPressed())
            {
                _holdTimers[i] += Time.deltaTime;
                
                if (_holdTimers[i] >= holdThreshold)
                    ShowTooltip(i);
            }

            if (action.WasReleasedThisFrame())
            {
                if (_holdTimers[i] < holdThreshold)
                    TryUseSkill(i);
                
                HideTooltip(i);
                _holdTimers[i] = 0f;
            }
        }
    }

    private void HandleManaRegen()
    {
        if (_data == null)
            return;

        _regenTimer += Time.deltaTime;

        if (_regenTimer < manaRegenDelay)
            return;

        if (_data.currentMana >= _data.@class.maxMana)
            return;
        
        _data.SetMana(_data.currentMana + manaRegenPerSecond * Time.deltaTime);
        _data.SetMana(Mathf.Min(_data.currentMana, _data.@class.maxMana));
    }
    
    public void TryUseSkill(int index)
    {
        var skill = _skills[index];
        if (!skill.IsReady)
            return;

        if (_data.currentMana < skill.Data.manaCost)
            return;
        
        UseSkill(skill);
    }
    
    private void UseSkill(SkillRuntime skill)
    {
        _data.SetMana(_data.currentMana - skill.Data.manaCost);
        _regenTimer = 0f;
        
        skill.Trigger();
        skillExecuter.Execute(skill.Data, _data);
        
        if (_animator)
            _animator.SetTrigger(AttackHash);
        
        onSkillUsed?.Invoke(skill);
    }

    private void ShowTooltip(int index)
    {
        if (_currentTooltipIndex == index)
            return;
        
        _currentTooltipIndex = index;
        
        TooltipUI.Instance?.ShowAt(_skills[index].Data, _hotbarUI.GetSlot(index));
    }

    private void HideTooltip(int index)
    {
        if (_currentTooltipIndex != index)
            return;

        _currentTooltipIndex = -1;

        TooltipUI.Instance?.Hide();
    }
    
    private void UpdateUI()
    {
        _hotbarUI.UpdateCooldowns(_skills);

        for (var i = 0; i < _skills.Length; i++)
        {
            var hasMana = _data.currentMana >= _skills[i].Data.manaCost;
            _hotbarUI.SetAvailable(i, hasMana);
        }
    }
}