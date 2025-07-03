using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

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

    public void LoadLevel(int levelIndex)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
            return;
        }

        if (levelIndex < 0 || levelIndex >= levelPrefabs.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        currentLevel = Instantiate(levelPrefabs[levelIndex], Vector3.zero, Quaternion.identity);
    }

    public void ReloadCurrentLevel()
    {
        if (currentLevel != null)
        {
            int index = levelPrefabs.IndexOf(currentLevel);
            LoadLevel(index);
        }
    }

}
