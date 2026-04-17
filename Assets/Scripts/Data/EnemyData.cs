using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;
    public EnemyType type;
    
    [Header("Prefab")]
    public Enemy enemyPrefab;
    public bool isBoss;
    
    [Header("Stats")]
    public float maxHealth;
    public float moveSpeed;
    
    [Header("Behavior")]
    public bool canSummon;
    
    [Header("Spawn")]
    public AnimationClip spawnAnimation;
    public GameObject spawnVFX;
    public float spawnDuration = 1.5f;
    
    [Header("Combat")]
    public EnemyAttackType attackType;
    public float attackDamage;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float damageDelay = 0.4f;
    public float aoeRadius = 3f;
    
    [Header("Ranged")]
    public Projectile projectilePrefab;
    public float projectileSpeed = 10f;
    
    [Header("Animation")]
    public AnimationClip attackAnimation;
    public float attackDuration = 1f;
}