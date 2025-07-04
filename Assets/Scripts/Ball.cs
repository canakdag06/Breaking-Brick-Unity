using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    //private void Update()
    //{
    //    Debug.Log(rb.linearVelocity.sqrMagnitude);
    //}

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleCollision(collision);
        }
    }

    private void HandlePaddleCollision(Collision2D collision)
    {
        Bounds paddleBounds = collision.collider.bounds;
        Vector2 contactPoint = collision.GetContact(0).point;
        float offset = (contactPoint.x - paddleBounds.center.x) / (paddleBounds.extents.x);
        Vector2 newDirection = new Vector2(offset, 1f).normalized;
        float currentSpeed = rb.linearVelocity.magnitude;
        rb.linearVelocity = newDirection * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("DeathWall"))
        {
            BallManager.Instance.ReturnBallToPool(this);
        }
    }
}
