using UnityEngine;
using System.Collections.Generic;

public static class EnemyRegistry
{
    private static readonly List<Enemy> Enemies = new();
    
    public static IReadOnlyList<Enemy> RegisteredEnemies => Enemies;

    public static void Register(Enemy enemy)
    {
        if (!Enemies.Contains(enemy))
            Enemies.Add(enemy);
    }

    public static void Unregister(Enemy enemy)
    {
        Enemies.Remove(enemy);
    }

    public static Enemy GetClosest(Vector3 position)
    {
        Enemy best = null;
        var bestDist = float.MaxValue;

        foreach (var e in Enemies)
        {
            if (!e)
                continue;

            var dist = Vector3.Distance(position, e.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = e;
            }
        }
        
        return best;
    }
}