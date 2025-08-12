using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyAnimations enemyAnimations;
    public GameObject enemyPrefab;

    private LevelInfo info;
    private Transform[] spawnPoints;

    private float spawnInterval;
    private float spawnTimer;

    private float maxEnemyCount;
    private float currentEnemyCount;
    private float minSpawnDelay;
    private float maxSpawnDelay;
    private float spawnDelayPercentage = 0.25f;



    void Update()
    {
        if (info == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && currentEnemyCount < maxEnemyCount)
        {
            SpawnEnemy();
            spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    public void SpawnEnemy()
    {
        EnemyType type = info.enemyType;

        var set = enemyAnimations.sprites
            .Find(s => s.enemyType == type);

        if (set == null)
        {
            Debug.LogError("Enemy animation set not found for type: " + type);
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        var enemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        var enemy = enemyGO.GetComponent<Enemy>();
        enemy.Initialize(set.animationSprites);
        enemy.SetMoveSpeed(info.enemyMoveSpeed);

        currentEnemyCount++;
    }

    public void InitializeEnemySpawner(LevelInfo levelInfo, GameObject levelInstance)
    {
        info = levelInfo;
        float offset = info.averageSpawnInterval * spawnDelayPercentage;

        minSpawnDelay = info.averageSpawnInterval - offset;     // randomise spawn delays
        maxSpawnDelay = info.averageSpawnInterval + offset;
        if (minSpawnDelay < 0f)
            minSpawnDelay = 0f;

        spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);

        maxEnemyCount = info.maxEnemiesOnScene;
        currentEnemyCount = 0;

        var markers = levelInstance.GetComponentsInChildren<SpawnPointMarker>();    // searching spawn points
        spawnPoints = new Transform[markers.Length];
        for (int i = 0; i < markers.Length; i++)
        {
            spawnPoints[i] = markers[i].transform;
        }
    }
}
