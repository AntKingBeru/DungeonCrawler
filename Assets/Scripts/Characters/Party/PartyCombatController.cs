using UnityEngine;
using System.Collections.Generic;

public class PartyCombatController : MonoBehaviour
{
    #region References
    
    [SerializeField] private CharacterController controller;
    [SerializeField] private PartyFollower follower;
    [SerializeField] private AISkillController skillController;
    [SerializeField] private SkillExecuter skillExecuter;
    
    #endregion
    
    #region Combat Settings
    
    [Header("Combat")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float combatMoveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Behavior")]
    [SerializeField] private float regroupDistance = 12f;
    [SerializeField] private float lowHealthThreshold = 0.3f;
    
    [Header("Targeting Weights")]
    [SerializeField] private float distanceWeight = 1f;
    [SerializeField] private float threatWeight = 2f;
    
    #endregion
    
    #region Stuck Handling
    
    [Header("Stuck")]
    [SerializeField] private float stuckThreshold = 0.1f;
    [SerializeField] private float stuckTime = 2f;
    
    private float _stuckTimer;
    
    #endregion
    
    #region Runtime
    
    private Transform _target;
    private bool _inCombat;

    private float _retargetTimer;
    private const float RetargetInterval = 0.5f;
    
    private CharacterSelectionData _data;

    private readonly Dictionary<Enemy, float> _threatTable = new();
    
    #endregion
    
    #region Initialize

    public void Initialize(CharacterSelectionData data, Animator animator)
    {
        _data = data;
        skillController.Initialize(data, animator);
    }
    
    #endregion
    
    #region Update

    private void Update()
    {
        HandleTargeting();
        HandleState();
    }
    
    #endregion
    
    #region Targeting

    private void HandleTargeting()
    {
        _retargetTimer -= Time.deltaTime;

        if (_retargetTimer > 0f)
            return;
        
        _retargetTimer = RetargetInterval;

        var enemies = EnemyRegistry.RegisteredEnemies;

        Enemy best = null;
        var bestScore = float.MinValue;

        foreach (var enemy in enemies)
        {
            if (!enemy || !enemy.isActiveAndEnabled)
                continue;
            
            var dist = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (dist > detectionRange)
                continue;

            var score = ScoreTarget(enemy, dist);

            if (score > bestScore)
            {
                bestScore = score;
                best = enemy;
            }
        }
        
        _target = best ? best.transform : null;
        _inCombat = _target;
    }

    private float ScoreTarget(Enemy enemy, float distance)
    {
        var score = 0f;
        
        score -= distance * distanceWeight;
        
        if (_threatTable.TryGetValue(enemy, out var threat))
            score += threat * threatWeight;
        
        return score;
    }
    
    #endregion
    
    #region State Handling
    
    private void HandleState()
    {
        if (_inCombat)
        {
            follower.enabled = false;
            HandleCombat();
        }
        else
        {
            follower.enabled = true;
        }
    }
    
    #endregion
    
    #region Combat Logic

    private void HandleCombat()
    {
        if (!_target)
            return;

        var direction = _target.position - transform.position;
        var distance = direction.magnitude;

        if (ShouldRetreat())
        {
            Regroup();
            return;
        }

        if (distance > attackRange)
        {
            Move(direction.normalized);
            HandleStuck();
        }
        else
        {
            FaceTarget(direction);
            TryAttack();
            _stuckTimer = 0f;
        }
    }

    private bool ShouldRetreat()
    {
        if (_data == null)
            return false;
        
        var hpPercent = _data.currentHealth / _data.@class.maxHealth;
        return hpPercent < lowHealthThreshold;
    }

    private void Regroup()
    {
        follower.enabled = true;
        
        var dist = Vector3.Distance(transform.position, follower.transform.position);
        
        if (dist > regroupDistance)
            follower.TeleportToFormation();
    }
    
    #endregion
    
    #region Movement

    private void Move(Vector3 direction)
    {
        if (skillExecuter && skillExecuter.IsRooted())
            return;
        
        var speed = combatMoveSpeed;

        if (skillExecuter)
            speed *= skillExecuter.GetMoveSpeedMultiplier();
        
        var velocity = direction * speed;
        controller.Move(velocity * Time.deltaTime);
        
        FaceTarget(direction);
    }

    private void FaceTarget(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    
    #endregion
    
    #region Stuck Handling

    private void HandleStuck()
    {
        if (controller.velocity.sqrMagnitude < stuckThreshold)
            _stuckTimer += Time.deltaTime;
        else
            _stuckTimer = 0f;
        
        if (_stuckTimer > stuckTime)
        {
            follower.TeleportToFormation();
            _stuckTimer = 0f;
        }
    }
    
    #endregion
    
    #region Combat Actions

    private void TryAttack()
    {
        if (!skillController || !_target)
            return;
        
        skillController.TryUseBestSkill(_target);
    }
    
    #endregion
    
    #region Threat System

    public void AddThreat(Enemy enemy, float amount)
    {
        _threatTable.TryAdd(enemy, 0f);

        _threatTable[enemy] += amount;
    }
    
    #endregion
}