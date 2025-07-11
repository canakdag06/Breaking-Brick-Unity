using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int poolSize = 10;

    private List<Ball> ballPool = new();
    private List<Ball> activeBalls = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializePool();
        SpawnInitialBall();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ballPrefab);
            obj.transform.parent = transform;
            obj.SetActive(false);
            Ball ball = obj.GetComponent<Ball>();
            ballPool.Add(ball);
        }
    }

    public void SpawnInitialBall()
    {
        ClearAllBalls();

        Ball ball = GetBallFromPool();
        Paddle.Instance.SetBall(ball);
        activeBalls.Add(ball);
    }

    public Ball GetBallFromPool()
    {
        foreach (Ball ball in ballPool)
        {
            if (!ball.gameObject.activeInHierarchy)
            {
                ball.gameObject.SetActive(true);
                return ball;
            }
        }
         return InstantiateNewBall();
    }

    public void ReturnBallToPool(Ball ball)
    {
        if (ball == null) return;

        ball.Stop();
        ball.gameObject.SetActive(false);
        ball.transform.SetParent(this.transform);
        activeBalls.Remove(ball);
    }

    public void ClearAllBalls()
    {
        foreach (Ball ball in activeBalls.ToArray())
        {
            ReturnBallToPool(ball);
        }

        activeBalls.Clear();
    }

    public bool HasActiveBalls()
    {
        return activeBalls.Count > 0 ? true : false;

        //if(activeBalls.Count == 0)
        //{
        //    return false;
        //}
        //else
        //{
        //    return true;
        //}
    }

    private Ball InstantiateNewBall()
    {
        GameObject obj = Instantiate(ballPrefab);
        obj.SetActive(false);
        Ball ball = obj.GetComponent<Ball>();
        ballPool.Add(ball);
        return ball;
    }

    // ======================= PowerUps =======================
    public void DuplicateBalls()
    {
        List<Ball> currentBalls = new(activeBalls);
        foreach (var originalBall in currentBalls)
        {
            Ball newBall = GetBallFromPool();
            newBall.transform.position = originalBall.transform.position;

            float angleOffset = Random.Range(-45f, 45f);
            Vector2 originalDir = originalBall.CurrentDirection;
            Vector2 newDir = Quaternion.Euler(0, 0, angleOffset) * originalDir;

            newBall.Launch(newDir, originalBall.CurrentSpeed);
            activeBalls.Add(newBall);
        }
    }

    public void EnableFlamingBall()
    {
        return;
    }
}
