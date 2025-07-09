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
                BallManager.Instance.DuplicateBalls();
                break;
            case PowerUpType.Expand:
                Paddle.Instance.ExpandPaddle();
                break;
            case PowerUpType.ExtraLife:
                GameManager.Instance.UpdateLives(true);
                break;
            case PowerUpType.FlamingBall:
                BallManager.Instance.EnableFlamingBall();
                break;
            case PowerUpType.Laser:
                Paddle.Instance.EnableLaser();
                break;
            case PowerUpType.Magnet:
                Paddle.Instance.EnableMagnet();
                break;
        }
    }
}
