using UnityEngine;

public class PartyCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private PartyFollower follower;
    [SerializeField] private AISkillController skillController;
    
    [Header("Combat")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float combatMoveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Stuck")]
    [SerializeField] private float stuckThreshold = 0.1f;
    [SerializeField] private float stuckTime = 2f;
    
    [Header("Performance")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int maxHits = 32;
    
    private Collider[] _overlapBuffer;
    
    private Transform _target;
    private bool _inCombat;

    private float _retargetTimer;
    private const float RetargetInterval = 0.5f;

    private float _stuckTimer;

    private void Awake()
    {
        _overlapBuffer = new Collider[maxHits];
    }

    public void Initialize(CharacterSelectionData data, Animator animator)
    {
        skillController.Initialize(data, animator);
    }

    private void Update()
    {
        HandleTargeting();
        HandleState();
    }

    private void HandleTargeting()
    {
        _retargetTimer -= Time.deltaTime;

        if (_retargetTimer > 0f)
            return;
        
        _retargetTimer = RetargetInterval;

        if (_target)
        {
            var distance = Vector3.Distance(transform.position, _target.position);

            if (distance <= detectionRange)
                return;
        }

        var hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            detectionRange,
            _overlapBuffer,
            enemyLayer
        );

        Transform bestTarget = null;
        var closestDist = float.MaxValue;

        for (var i = 0; i < hitCount; i++)
        {
            var col = _overlapBuffer[i];
            
            if (!col.TryGetComponent<Enemy>(out var enemy))
                continue;
            
            if (!enemy.isActiveAndEnabled)
                continue;
            
            var dist = Vector3.Distance(transform.position, col.transform.position);
            
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = col.transform;
            }
        }
        
        _target = bestTarget;
        _inCombat = _target;
    }
    
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

    private void HandleCombat()
    {
        if (!_target)
            return;

        var direction = _target.position - transform.position;
        var distance = direction.magnitude;

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

    private void Move(Vector3 direction)
    {
        var velocity = direction * combatMoveSpeed;
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

    private void TryAttack()
    {
        if (!skillController || !_target)
            return;
        
        skillController.TryUseBestSkill(_target);
    }
}