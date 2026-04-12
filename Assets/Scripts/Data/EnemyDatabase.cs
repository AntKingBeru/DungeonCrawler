using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    [Header("Normal Enemies")]
    public EnemyData skeletonMinion;
    public EnemyData skeletonWarrior;
    public EnemyData skeletonKnight;
    public EnemyData skeletonRogue;
    public EnemyData skeletonMage;
    
    [Header("Boss Enemies")]
    public EnemyData skeletonGolemAxe;
    public EnemyData skeletonGolemMace;
    public EnemyData necromancer;

    public EnemyData GetRandomNormal()
    {
        var list = new[]
        {
            skeletonMinion,
            skeletonWarrior,
            skeletonKnight,
            skeletonRogue,
            skeletonMage
        };
        
        return list[Random.Range(0, list.Length)];
    }

    public EnemyData GetRandomMiniBoss()
    {
        var list = new[]
        {
            skeletonGolemAxe,
            skeletonGolemMace
        };
        
        return list[Random.Range(0, list.Length)];
    }
}