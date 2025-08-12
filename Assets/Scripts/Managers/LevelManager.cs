using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public EnemySpawner enemySpawner;
    public int CurrentLevelIndex { get; private set; } = -1;

    [Header("Level Data")]
    [SerializeField] private List<LevelInfo> levels;


    private GameObject currentLevel;
    public LevelInfo CurrentLevelInfo { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LoadLevel()
    {
        //if (currentLevel != null)
        //{
        //    Destroy(currentLevel);
        //}

        CurrentLevelIndex++;

        if (CurrentLevelIndex < 0 || CurrentLevelIndex >= levels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        CurrentLevelInfo = levels[CurrentLevelIndex];
        currentLevel = Instantiate(CurrentLevelInfo.levelPrefab, Vector3.zero, Quaternion.identity);
        enemySpawner.InitializeEnemySpawner(CurrentLevelInfo, currentLevel);
    }

    public void ReloadCurrentLevel()
    {
        if (currentLevel != null)
        {
            CurrentLevelIndex--;
            LoadLevel();
        }
    }

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