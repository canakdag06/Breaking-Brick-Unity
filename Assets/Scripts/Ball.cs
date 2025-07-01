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
            // Paddle'ýn geniþliðini ve merkezini al
            Bounds paddleBounds = collision.collider.bounds;

            // Çarpma noktasýný al (ilk temasý baz alýyoruz)
            Vector2 contactPoint = collision.GetContact(0).point;

            // Paddle merkezine göre ne kadar saða veya sola çarptýðýný hesapla (-1 ile 1 arasýnda bir deðer)
            float offset = (contactPoint.x - paddleBounds.center.x) / (paddleBounds.extents.x);

            // Y ekseni yukarýya sabit, X ekseni paddle'daki çarpma noktasýna göre
            Vector2 newDirection = new Vector2(offset, 1f).normalized;

            // Topun hýzýný koruyarak yönü güncelle
            float currentSpeed = rb.linearVelocity.magnitude;
            rb.linearVelocity = newDirection * currentSpeed;
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        // Eðer top neredeyse yatay gidiyorsa, biraz yukarý it
        if (Mathf.Abs(velocity.y) < 0.1f)
        {
            velocity.y = Mathf.Sign(velocity.y) == 0 ? 0.5f : Mathf.Sign(velocity.y) * 0.5f;
        }

        // Eðer top neredeyse dikey gidiyorsa, biraz yana eð
        if (Mathf.Abs(velocity.x) < 0.1f)
        {
            velocity.x = Mathf.Sign(velocity.x) == 0 ? 0.5f : Mathf.Sign(velocity.x) * 0.5f;
        }

        rb.linearVelocity = velocity.normalized * speed;
    }
}
