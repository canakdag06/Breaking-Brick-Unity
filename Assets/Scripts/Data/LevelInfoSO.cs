using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfoSO", menuName = "Level/LevelInfoSO")]
public class LevelInfoSO : ScriptableObject
{
    public List<LevelInfo> levels;
}

[System.Serializable]
public class LevelInfo
{
    public GameObject levelPrefab;

    [Header("Enemy Settings")]
    public EnemyType enemyType;
    public float averageSpawnInterval = 15f;
    public float enemyMoveSpeed = 0.5f;
    public int maxEnemiesOnScene = 2;
}