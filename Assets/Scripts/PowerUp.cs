using UnityEngine;

public enum PowerUpType
{
    DuplicateBall,
    Expand,
    ExtraLife,
    FlamingBall,
    Laser,
    Magnet
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float fallSpeed = 2f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Paddle"))
        {
            //PowerUpManager.Instance.ApplyPowerUp(type);
        }
        Destroy(gameObject);
    }
}
