using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;
    public EnemyType type;
    
    [Header("Prefab")]
    public Enemy enemyPrefab;
    
    [Header("Stats")]
    public float maxHealth;
    public float damage;
    public float moveSpeed;
    
    [Header("Behavior")]
    public bool canSummon;
    
    [Header("Spawn")]
    public AnimationClip spawnAnimation;
    public GameObject spawnVFX;
    public float spawnDuration = 1.5f;
    
    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDamageMultiplier = 1f;
    
    [Header("Animation")]
    public AnimationClip attackAnimation;
    public float attackDuration = 1f;
}