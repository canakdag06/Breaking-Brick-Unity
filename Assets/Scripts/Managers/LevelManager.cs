using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public EnemySpawner enemySpawner;
    public int CurrentLevelIndex { get; private set; } = -1;

    [Header("Levels")]
    [SerializeField] private List<GameObject> levelPrefabs;

    private GameObject currentLevel;

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

        if (CurrentLevelIndex < 0 || CurrentLevelIndex >= levelPrefabs.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        currentLevel = Instantiate(levelPrefabs[CurrentLevelIndex], Vector3.zero, Quaternion.identity);
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
