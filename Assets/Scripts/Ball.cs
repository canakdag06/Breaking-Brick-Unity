using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector2 CurrentDirection => rb.linearVelocity.normalized;
    public float CurrentSpeed => rb.linearVelocity.magnitude;

    public bool IsLaunched { get; private set; }

    [SerializeField] private ParticleSystem flamingParticle;

    private Rigidbody2D rb;
    private float speed = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
        IsLaunched = true;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        IsLaunched = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal;
            //Debug.Log("normal: " + normal);

            if (normal.y > 0.5f)    // the ball hits the top of the Paddle
            {
                HandlePaddleCollision(collision);
            }
            else
            {
                FixBallDirection();
            }

            AudioManager.Instance.PlaySFX(SoundType.ImpactPaddle);
        }
        else if (collision.gameObject.CompareTag("DeathWall"))
        {
            BallManager.Instance.ReturnBallToPool(this);
            if (!BallManager.Instance.HasActiveBalls())
            {
                GameManager.Instance.UpdateLives(false);
            }
        }
        else
        {
            FixBallDirection();
            if(collision.gameObject.CompareTag("Wall"))
            {
                AudioManager.Instance.PlaySFX(SoundType.ImpactWall);
            }
        }
    }

    private void FixBallDirection()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speed;

        Vector2 v = rb.linearVelocity.normalized;
        if (Mathf.Abs(v.x) < 0.01f)
        {
            v.x = 0.1f * Mathf.Sign(Random.Range(-1f, 1f));
        }
        if (Mathf.Abs(v.y) < 0.01f)
        {
            v.y = 0.1f * Mathf.Sign(Random.Range(-1f, 1f));
        }
        rb.linearVelocity = v.normalized * speed;
    }

    private void HandlePaddleCollision(Collision2D collision)
    {
        if (Paddle.Instance.TryMagnetAttach(this))  // Magnet power-up check
        {
            return;
        }

        Bounds paddleBounds = collision.collider.bounds;
        Vector2 contactPoint = collision.GetContact(0).point;
        float offset = (contactPoint.x - paddleBounds.center.x) / (paddleBounds.extents.x);
        Vector2 newDirection = new Vector2(offset, 1f).normalized;
        float currentSpeed = rb.linearVelocity.magnitude;
        rb.linearVelocity = newDirection * currentSpeed;
    }

    public void SetFlaming(bool value)
    {
        if (value)
            flamingParticle.Play();
        else
            flamingParticle.Stop();
    }
}
