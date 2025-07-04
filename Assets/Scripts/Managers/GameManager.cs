using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> OnLifeChanged;

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
    }

    private void Start()
    {
        StartGame();
        OnLifeChanged?.Invoke(lives);
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
        }
        OnLifeChanged?.Invoke(lives);
    }

    private void StartGame()
    {
        isPaused = false;
        isGameOver = false;
        Time.timeScale = 1f;
    }
}
