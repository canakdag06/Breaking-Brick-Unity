using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }
    public float ExpandDuration => expandDuration;
    public float FlamingBallDuration => flamingBallDuration;
    public float LaserDuration => laserDuration;
    public float MagnetDuration => magnetDuration;

    [SerializeField] private float expandDuration = 20f;
    [SerializeField] private float flamingBallDuration = 10f;
    [SerializeField] private float laserDuration = 10f;
    [SerializeField] private float magnetDuration = 10f;

    private Paddle paddle;
    private BallManager ballManager;
    private GameManager gameManager;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        paddle = Paddle.Instance;
        ballManager = BallManager.Instance;
        gameManager = GameManager.Instance;
    }

    public void ApplyPowerUp(PowerUpType type)
    {
        switch(type)
        {
            case PowerUpType.DuplicateBall:
                ApplyDuplicateBalls();
                break;
            case PowerUpType.Expand:
                ApplyExpandPaddle();
                break;
            case PowerUpType.ExtraLife:
                ApplyExtraLife();
                break;
            case PowerUpType.FlamingBall:
                ApplyFlamingBall();
                break;
            case PowerUpType.Laser:
                ApplyLaser();
                break;
            case PowerUpType.Magnet:
                ApplyMagnet();
                break;
        }
    }

    public void ResetPowerUps()
    {
        paddle.ShrinkAfterDelay(3f);
        paddle.DisableMagnet();
        paddle.LaserTimer(0f);

        //BallManager.Instance.DisableFlamingBall();
    }

    private void ApplyDuplicateBalls()
    {
        ballManager.DuplicateBalls();
    }

    private void ApplyExpandPaddle()
    {
        paddle.ExpandPaddle(expandDuration);
    }

    private void ApplyExtraLife()
    {
        gameManager.UpdateLives(true);
    }

    private void ApplyFlamingBall()
    {
        ballManager.EnableFlamingBall(flamingBallDuration);
    }

    private void ApplyLaser()
    {
        paddle.EnableLaser(laserDuration);
    }

    private void ApplyMagnet()
    {
        paddle.EnableMagnet(magnetDuration);
    }
}