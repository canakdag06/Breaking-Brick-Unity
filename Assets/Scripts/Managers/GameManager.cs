using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> OnLifeChanged;
    public event Action OnLifeLostEffectRequested;

    public int LastPlayedLevel { get; private set; }
    public int HighestLevelReached { get; private set; }
    public int Score { get; private set; }

    public int Lives => lives;
    public bool IsPaused => isPaused;
    public bool IsGameOver => isGameOver;

    private int lives = 3;
    private bool isPaused = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    private void Start()
    {
        //LevelManager.Instance.LoadLevel();
        //StartCoroutine(StartGameAfterLevelInfo());
        OnLifeChanged?.Invoke(lives);
    }

    private IEnumerator StartGameAfterLevelInfo()
    {
        yield return StartCoroutine(UIManager.Instance.ShowMessage("BREAK THEM ALL!"));

        StartGame();
    }

    public void UpdateLives(bool isAdd)
    {
        if (isAdd)
        {
            lives++;
        }
        else
        {
            lives--;
            if (lives <= 0)
            {
                HandleGameOver();
            }
            OnLifeLostEffectRequested?.Invoke();
        }
        OnLifeChanged?.Invoke(lives);
    }

    private void HandleGameOver()
    {
        throw new NotImplementedException();
    }

    private void StartGame()
    {
        isPaused = false;
        isGameOver = false;
        Time.timeScale = 1f;
    }

    public void SaveProgress(int levelIndex)
    {
        LastPlayedLevel = levelIndex;
        if (HighestLevelReached < levelIndex)
            HighestLevelReached = levelIndex;

        PlayerPrefs.SetInt("LastPlayedLevel", LastPlayedLevel);
        PlayerPrefs.SetInt("HighestLevel", HighestLevelReached);
        PlayerPrefs.SetInt("Score", ScoreManager.Instance.GetScore());
        PlayerPrefs.SetInt("Lives", lives);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        LastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);
        HighestLevelReached = PlayerPrefs.GetInt("HighestLevel", 0);
        Score = PlayerPrefs.GetInt("Score", 0);
        lives = PlayerPrefs.GetInt("Lives", 3);

        OnLifeChanged?.Invoke(lives);
    }
}
