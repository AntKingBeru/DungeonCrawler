using UnityEngine;
using System.Collections;

public class Necromancer : Enemy
{
    [Header("Summon")]
    [SerializeField] private EnemyDatabase database;
    [SerializeField] private float summonInterval = 5f;
    
    [Header("Ranged Attack")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float rangedDamage = 18f;
    [SerializeField] private float rangedCooldown = 3.5f;
    [SerializeField] private float rangedRange = 8f;
    [SerializeField] private float projectileSpeed = 10f;
    
    private float _summonTimer;
    private float _rangedTimer;

    protected override void Update()
    {
        if (!IsActive)
            return;

        AttackTimer -= Time.deltaTime;
        _rangedTimer -= Time.deltaTime;
        
        var currentTarget = ForcedTarget ? ForcedTarget : Target;
        
        if (!currentTarget)
            return;
        
        var dist = Vector3.Distance(transform.position, Target.position);

        if (!IsAttacking && _summonTimer >= summonInterval)
        {
            _summonTimer += Time.deltaTime;

            if (_summonTimer >= summonInterval)
            {
                _summonTimer = 0f;
                Summon();
            }
        }

        if (dist <= Data.attackRange)
            TryAttack(currentTarget);
        else if (dist <= rangedRange)
            HandleRangedAttack();
        else
            MoveTo(currentTarget);
        
        UpdateAnimation();
    }

    private void Summon()
    {
        var data = database.GetRandomNormal();

        var offset = Random.insideUnitSphere * 3f;
        offset.y = 0;
        
        var spawnPos = transform.position + offset;

        var enemy = Instantiate(
            data.enemyPrefab,
            spawnPos,
            Quaternion.identity
        );

        enemy.Initialize(data, Target);
    }

    private void HandleRangedAttack()
    {
        if (IsAttacking || _rangedTimer > 0f)
            return;
        
        agent.isStopped = true;
        
        LookAtTarget();

        StartCoroutine(RangedAttackRoutine());
    }

    private IEnumerator RangedAttackRoutine()
    {
        IsAttacking = true;
        
        if (animator && Data.attackAnimation)
            animator.SetTrigger(AttackHash);
        
        yield return new WaitForSeconds(Data.damageDelay);

        FireProjectile();
        
        yield return new WaitForSeconds(Data.attackDuration - Data.damageDelay);
        
        _rangedTimer = rangedCooldown;
        IsAttacking = false;
    }

    private void FireProjectile()
    {
        if (!Target || !projectilePrefab)
            return;

        var dir = (Target.position - transform.position).normalized;

        var proj = Instantiate(
            projectilePrefab,
            transform.position + Vector3.up * 1.5f,
            Quaternion.LookRotation(dir)
        );
        
        proj.Initialize(Target, rangedDamage, projectileSpeed);
    }

    private void LookAtTarget()
    {
        var dir = (Target.position - transform.position).normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }
}