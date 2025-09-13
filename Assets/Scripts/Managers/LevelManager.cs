using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public EnemySpawner enemySpawner;
    public LevelInfo CurrentLevelInfo { get; private set; }
    public int CurrentLevelIndex { get; private set; } = -1;

    public static event Action OnLevelFinished;

    [Header("Level Data")]
    [SerializeField] private LevelInfoSO levelDatabase;
    //[SerializeField] private List<LevelInfo> levels;

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

    private void Start()
    {
        LoadLevel();
    }

    private void OnDisable()
    {
        Brick.OnBrickBreaks -= HandleBrickBreaks;
    }

    public void LoadLevel()
    {
        if (CurrentLevelIndex < 0)
            CurrentLevelIndex = GameManager.Instance.LastPlayedLevel;

        if (CurrentLevelIndex >= levelDatabase.levels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        CurrentLevelInfo = levelDatabase.levels[CurrentLevelIndex];
        Destroy(currentLevel);
        currentLevel = Instantiate(CurrentLevelInfo.levelPrefab, Vector3.zero, Quaternion.identity);
        enemySpawner.InitializeEnemySpawner(CurrentLevelInfo, currentLevel);

        CheckBrickCount();
        StartCoroutine(StartLevelSequence());

        //Debug.Log("LastPlayedLevel: " + GameManager.Instance.LastPlayedLevel);
        //Debug.Log("HighestLevelReached: " + GameManager.Instance.HighestLevelReached);
        //Debug.Log("currentLevelIndex: " + CurrentLevelIndex);
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

        if (totalBricks == 0)
        {
            OnLevelFinished?.Invoke();
            StartCoroutine(FinishLevelSequence());
        }
    }

    private IEnumerator FinishLevelSequence()
    {
        BallManager.Instance.ClearAllBalls();
        PowerUpManager.Instance.ResetAndDestroyPowerUps();
        yield return UIManager.Instance.ShowMessage("IS COMPLETED!");
        yield return UIManager.Instance.FadeOut(1f);


        if (CurrentLevelIndex >= levelDatabase.levels.Count - 1)
        {
            yield return UIManager.Instance.ShowMessage("GAME COMPLETED! THANKS FOR PLAYING");
            GameManager.Instance.SetLastPlayedLevel(levelDatabase.levels.Count - 1);
            //Debug.Log("LastPlayedLevel: " + GameManager.Instance.LastPlayedLevel);
            //Debug.Log("HighestLevelReached: " + GameManager.Instance.HighestLevelReached);
            SceneManager.LoadScene("MainMenu");
            yield break;
        }



        CurrentLevelIndex++;
        GameManager.Instance.SaveProgress(CurrentLevelIndex);
        LoadLevel();
    }

    private IEnumerator StartLevelSequence()
    {
        yield return UIManager.Instance.FadeIn(1f);
        StartCoroutine(UIManager.Instance.ShowMessage("BREAK THEM ALL!"));
        yield return new WaitForSeconds(1f);
        BallManager.Instance.SpawnInitialBall();
        enemySpawner.ResetTimer();
    }
}