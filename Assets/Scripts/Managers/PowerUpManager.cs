using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [SerializeField] private float flamingBallDuration = 10f;
    [SerializeField] private float laserDuration = 10f;

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

    private void ApplyDuplicateBalls()
    {
        BallManager.Instance.DuplicateBalls();
    }

    private void ApplyExpandPaddle()
    {
        Paddle.Instance.ExpandPaddle();
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
        Debug.Log("MAGNET APLIED");
        Paddle.Instance.EnableMagnet();
    }
}
