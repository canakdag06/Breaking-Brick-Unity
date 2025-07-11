using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

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
        Debug.Log("DUPLICATE BALLS APLIED");
        BallManager.Instance.DuplicateBalls();
    }

    private void ApplyExpandPaddle()
    {
        Debug.Log("EXPAND PADDLE APLIED");
        Paddle.Instance.ExpandPaddle();
    }

    private void ApplyExtraLife()
    {
        Debug.Log("EXTRA LIFE APLIED");
        GameManager.Instance.UpdateLives(true);
    }

    private void ApplyFlamingBall()
    {
        Debug.Log("FLAMING BALL APLIED");
        BallManager.Instance.EnableFlamingBall();
    }

    private void ApplyLaser()
    {
        Debug.Log("LASER APLIED");
        Paddle.Instance.EnableLaser();
    }

    private void ApplyMagnet()
    {
        Debug.Log("MAGNET APLIED");
        Paddle.Instance.EnableMagnet();
    }
}
