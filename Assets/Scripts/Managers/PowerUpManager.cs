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


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        Paddle.Instance.ShrinkAfterDelay(3f);
        Debug.Log("RESET EXPAND PADDLE POWERUP");
        Paddle.Instance.DisableMagnet();
        //Paddle.Instance.DisableLaser();

        //BallManager.Instance.DisableFlamingBall();
    }

    private void ApplyDuplicateBalls()
    {
        BallManager.Instance.DuplicateBalls();
    }

    private void ApplyExpandPaddle()
    {
        Paddle.Instance.ExpandPaddle(expandDuration);
    }

    private void ApplyExtraLife()
    {
        GameManager.Instance.UpdateLives(true);
    }

    private void ApplyFlamingBall()
    {
        BallManager.Instance.EnableFlamingBall(flamingBallDuration);
    }

    private void ApplyLaser()
    {
        Paddle.Instance.EnableLaser(laserDuration);
    }

    private void ApplyMagnet()
    {
        Paddle.Instance.EnableMagnet(magnetDuration);
    }
}