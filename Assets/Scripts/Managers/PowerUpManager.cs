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
                ballManager.DuplicateBalls();
                break;
            case PowerUpType.Expand:
                paddle.ExpandPaddle(expandDuration);
                break;
            case PowerUpType.ExtraLife:
                gameManager.UpdateLives(true);
                break;
            case PowerUpType.FlamingBall:
                ballManager.EnableFlamingBall(flamingBallDuration);
                break;
            case PowerUpType.Laser:
                paddle.EnableLaser(laserDuration);
                break;
            case PowerUpType.Magnet:
                paddle.EnableMagnet(magnetDuration);
                break;
        }
    }

    public void ResetAndDestroyPowerUps()
    {
        paddle.ShrinkAfterDelay(3f);
        paddle.DisableMagnet();
        paddle.LaserTimer(1.5f);

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}