using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }

    private List<Ball> balls = new();

    [SerializeField] private Ball ballPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        SpawnInitialBall();
    }
    public void SpawnInitialBall()
    {
        ClearBalls();
        Ball newBall = Instantiate(ballPrefab, Paddle.Instance.ballLocation.position, Quaternion.identity);
        Paddle.Instance.SetBall(newBall);
        AddBall(newBall);
    }

    public void ClearBalls()
    {
        foreach (var ball in balls)
        {
            if (ball != null)
                Destroy(ball.gameObject);
        }
        balls.Clear();
    }

    public void AddBall(Ball newBall)
    {
        if(!balls.Contains(newBall))
        {
            balls.Add(newBall);
        }
    }


}
