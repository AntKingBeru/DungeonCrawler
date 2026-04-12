using UnityEngine;

public class Necromancer : Enemy
{
    [Header("Summon")]
    [SerializeField] private EnemyDatabase database;
    [SerializeField] private float summonInterval = 5f;
    
    private float _summonTimer;

    protected override void Update()
    {
        base.Update();

        if (!IsActive || IsAttacking)
            return;

        _summonTimer += Time.deltaTime;

        if (_summonTimer >= summonInterval)
        {
            _summonTimer = 0f;
            Summon();
        }
    }

    private void Summon()
    {
        var data = database.GetRandomNormal();

        var offset = Random.insideUnitSphere * 3f;
        offset.y = 0;

        var enemy = Instantiate(
            data.enemyPrefab,
            transform.position + offset,
            Quaternion.identity
        );

        enemy.Initialize(data, Target);
    }
}