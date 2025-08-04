using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;

    [Range(0f, 30f)] public float maxBounceAngleOffset = 15f;
    [Range(0f, 10f)] public float wiggleAmplitude = 5f;
    [Range(0f, 10f)] public float wiggleFrequency = 2f;

    private Vector2 direction;
    private float directionChangeCooldown;

    void Start()
    {
        direction = Random.insideUnitCircle.normalized;
        directionChangeCooldown = Random.Range(2f, 5f);
    }

    void Update()
    {
        directionChangeCooldown -= Time.deltaTime;
        if (directionChangeCooldown <= 0f)
        {
            direction = Random.insideUnitCircle.normalized;
            directionChangeCooldown = Random.Range(2f, 5f);
        }

        float wiggle = Mathf.Sin(Time.time * wiggleFrequency) * wiggleAmplitude;
        Vector2 wiggledDirection = Quaternion.Euler(0, 0, wiggle) * direction;

        transform.Translate(wiggledDirection.normalized * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.GetContact(0);

            Vector2 normal = contact.normal;
            Vector2 reflected = Vector2.Reflect(direction, normal);

            float angleOffset = Random.Range(-maxBounceAngleOffset, maxBounceAngleOffset);
            reflected = Quaternion.Euler(0, 0, angleOffset) * reflected;

            direction = reflected.normalized;
        }
    }
}
