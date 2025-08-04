using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float directionChangeInterval = 1f;
    public float noiseStrength = 0.5f;
    public float raycastCheckDistance = 1f;

    private Vector2 currentDirection;
    private float directionTimer;
    private Transform paddleTransform;
    private Rigidbody2D rb;

    // Engel layer'lar�n� belirle
    public LayerMask obstacleLayers;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        paddleTransform = GameObject.FindWithTag("Paddle").transform;

        PickNewDirection();
        directionTimer = directionChangeInterval;
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0)
        {
            PickNewDirection();
            directionTimer = directionChangeInterval;
        }

        rb.linearVelocity = currentDirection * moveSpeed;
    }

    void PickNewDirection()
    {
        // Paddle y�n�ne rastgele sapma ekle
        Vector2 toPaddle = (paddleTransform.position - transform.position).normalized;
        Vector2 noise = Random.insideUnitCircle.normalized * noiseStrength;
        Vector2 preferred = (toPaddle + noise).normalized;

        // E�er yol a��ksa paddle y�n�ne sapmal� git
        if (IsDirectionClear(preferred))
        {
            currentDirection = preferred;
            return;
        }

        // De�ilse alternatif y�n bulmaya �al��
        TryFindClearDirection();
    }

    bool IsDirectionClear(Vector2 direction)
    {
        Debug.Log("RAYCAST");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastCheckDistance, obstacleLayers);
        Debug.DrawRay(transform.position, direction * raycastCheckDistance, Color.yellow, 0.1f);
        return hit.collider == null;
    }

    void TryFindClearDirection()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            if (IsDirectionClear(randomDirection))
            {
                currentDirection = randomDirection;
                return;
            }
        }

        // Son �are: tamamen rastgele git
        currentDirection = Random.insideUnitCircle.normalized;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Y�n� hemen de�i�tir ama �nce fiziksel olarak biraz geri it (�arp��madan ��kart)
        //Vector2 normal = collision.contacts[0].normal;
        //transform.position += (Vector3)normal * 0.05f;

        PickNewDirection();
        directionTimer = directionChangeInterval;
    }
}
