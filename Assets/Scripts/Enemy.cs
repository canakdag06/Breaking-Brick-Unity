using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float directionChangeInterval;
    public float noiseStrength;
    public float raycastCheckDistance;

    private Vector2 currentDirection;
    private float directionTimer;
    private Transform paddleTransform;
    private Rigidbody2D rb;

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
        Vector2 toPaddle = (paddleTransform.position - transform.position).normalized;
        Vector2 noise = Random.insideUnitCircle.normalized * noiseStrength;
        Vector2 preferred = (toPaddle + noise).normalized;

        if (IsDirectionClear(preferred))
        {
            currentDirection = preferred;
            return;
        }

        TryFindClearDirection();
    }

    bool IsDirectionClear(Vector2 direction)
    {
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

        currentDirection = Random.insideUnitCircle.normalized;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PickNewDirection();
        directionTimer = directionChangeInterval;
    }
}
