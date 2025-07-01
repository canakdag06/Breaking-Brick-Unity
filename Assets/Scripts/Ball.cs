using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 2f;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
        if (collision.gameObject.CompareTag("Paddle"))
        {
            // Paddle'�n geni�li�ini ve merkezini al
            Bounds paddleBounds = collision.collider.bounds;

            // �arpma noktas�n� al (ilk temas� baz al�yoruz)
            Vector2 contactPoint = collision.GetContact(0).point;

            // Paddle merkezine g�re ne kadar sa�a veya sola �arpt���n� hesapla (-1 ile 1 aras�nda bir de�er)
            float offset = (contactPoint.x - paddleBounds.center.x) / (paddleBounds.extents.x);

            // Y ekseni yukar�ya sabit, X ekseni paddle'daki �arpma noktas�na g�re
            Vector2 newDirection = new Vector2(offset, 1f).normalized;

            // Topun h�z�n� koruyarak y�n� g�ncelle
            float currentSpeed = rb.linearVelocity.magnitude;
            rb.linearVelocity = newDirection * currentSpeed;
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        // E�er top neredeyse yatay gidiyorsa, biraz yukar� it
        if (Mathf.Abs(velocity.y) < 0.1f)
        {
            velocity.y = Mathf.Sign(velocity.y) == 0 ? 0.5f : Mathf.Sign(velocity.y) * 0.5f;
        }

        // E�er top neredeyse dikey gidiyorsa, biraz yana e�
        if (Mathf.Abs(velocity.x) < 0.1f)
        {
            velocity.x = Mathf.Sign(velocity.x) == 0 ? 0.5f : Mathf.Sign(velocity.x) * 0.5f;
        }

        rb.linearVelocity = velocity.normalized * speed;
    }
}
