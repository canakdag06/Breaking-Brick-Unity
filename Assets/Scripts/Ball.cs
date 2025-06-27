using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
