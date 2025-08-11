using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyAnimations enemyAnimations;
    public GameObject enemyPrefab;

    private LevelInfo info;

    private float spawnInterval;
    private float spawnTimer;

    private float maxEnemyCount;
    private float currentEnemyCount;


    void Start()
    {
        info = LevelManager.Instance.CurrentLevelInfo;

        spawnInterval = info.enemySpawnInterval;
        spawnTimer = spawnInterval;
        maxEnemyCount = info.maxEnemiesOnScene;
        currentEnemyCount = 0;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && currentEnemyCount < maxEnemyCount)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    public void SpawnEnemy(Vector3 position)
    {
        EnemyType type = info.enemyType;

        var set = enemyAnimations.sprites
            .Find(s => s.enemyType == type);

        if (set == null)
        {
            Debug.LogError("Enemy animation set not found for type: " + type);
            return;
        }

        var enemyGO = Instantiate(enemyPrefab, position, Quaternion.identity);
        var enemy = enemyGO.GetComponent<Enemy>();
        enemy.Initialize(set.animationSprites);
        enemy.SetMoveSpeed(info.enemyMoveSpeed);

        currentEnemyCount++;
    }
}
