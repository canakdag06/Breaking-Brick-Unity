using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public EnemySpawner enemySpawner;

    [Header("Levels")]
    [SerializeField] private List<GameObject> levelPrefabs;

    private GameObject currentLevel;
    private int currentLevelIndex = -1;

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

        currentLevelIndex++;

        if (currentLevelIndex < 0 || currentLevelIndex >= levelPrefabs.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        currentLevel = Instantiate(levelPrefabs[currentLevelIndex], Vector3.zero, Quaternion.identity);
    }

    public void ReloadCurrentLevel()
    {
        if (currentLevel != null)
        {
            currentLevelIndex--;
            LoadLevel();
        }
    }

}
