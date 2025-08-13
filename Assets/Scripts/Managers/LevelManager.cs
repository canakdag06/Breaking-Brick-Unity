using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public EnemySpawner enemySpawner;
    public LevelInfo CurrentLevelInfo { get; private set; }
    public int CurrentLevelIndex { get; private set; } = -1;

    [Header("Level Data")]
    [SerializeField] private List<LevelInfo> levels;

    private GameObject currentLevel;
    private int totalBricks;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        Brick.OnBrickBreaks += HandleBrickBreaks;
    }

    private void OnDisable()
    {
        Brick.OnBrickBreaks -= HandleBrickBreaks;
    }

    public void LoadLevel()
    {
        CurrentLevelIndex++;

        if (CurrentLevelIndex < 0 || CurrentLevelIndex >= levels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        CurrentLevelInfo = levels[CurrentLevelIndex];
        currentLevel = Instantiate(CurrentLevelInfo.levelPrefab, Vector3.zero, Quaternion.identity);
        enemySpawner.InitializeEnemySpawner(CurrentLevelInfo, currentLevel);

        CheckBrickCount();
        StartCoroutine(StartLevelSequence());
    }

    public void ReloadCurrentLevel()
    {
        if (currentLevel != null)
        {
            CurrentLevelIndex--;
            LoadLevel();
        }
    }

    private void CheckBrickCount()
    {
        totalBricks = 0;
        Brick[] bricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
        foreach (Brick brick in bricks)
        {
            if (brick.IsBreakable)
            {
                totalBricks++;
            }
        }
    }

    private void HandleBrickBreaks()
    {
        totalBricks--;

        if(totalBricks == 0)
        {
            StartCoroutine(FinishLevelSequence());
        }
    }

    private IEnumerator FinishLevelSequence()
    {
        BallManager.Instance.SpawnInitialBall();    // it has ClearAllBalls() in it


        yield return UIManager.Instance.ShowMessage("IS COMPLETED!");
        yield return UIManager.Instance.FadeOut(1f);
        LoadLevel();
    }

    private IEnumerator StartLevelSequence()
    {
        yield return UIManager.Instance.FadeIn(1f);
        yield return StartCoroutine(UIManager.Instance.ShowMessage("BREAK THEM ALL!"));
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