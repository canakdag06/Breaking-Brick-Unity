using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float directionChangeInterval;
    public float noiseStrength;
    public float raycastCheckDistance;

    public LayerMask obstacleLayers;

    private Vector2 currentDirection;
    private float directionTimer;
    private Transform paddleTransform;
    private Rigidbody2D rb;
    private bool canMove = false;

    private Sprite[] sprites;
    private float frameRate;
    private int currentFrame;
    private float animationTimer;
    private SpriteRenderer sr;
    private float baseMoveSpeed = 0.5f;
    private float baseFrameRate = 1f;



    private void OnEnable()
    {
        SpawnEffect();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        paddleTransform = GameObject.FindWithTag("Paddle").transform;

        PickNewDirection();
        directionTimer = directionChangeInterval;
    }

    public void Initialize(Sprite[] animationSprites)
    {
        sprites = animationSprites;
        frameRate = baseFrameRate * (baseMoveSpeed / moveSpeed);
        sr = GetComponent<SpriteRenderer>();
        currentFrame = 0;
        animationTimer = 0f;
    }

    void Update()
    {
        LoopAnimation();

        if (!canMove)
        {
            return;
        }
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0)
        {
            PickNewDirection();
            directionTimer = directionChangeInterval;
        }
        Vector2 targetVelocity = currentDirection * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.deltaTime * 5f);
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }

    private void LoopAnimation()
    {
        if (sprites == null || sprites.Length == 0) return;

        animationTimer += Time.deltaTime;
        if (animationTimer >= frameRate)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % sprites.Length;
            sr.sprite = sprites[currentFrame];
        }
    }

    private void SpawnEffect()
    {
        transform.localScale = Vector3.zero;
        Color startColor = spriteRenderer.color;
        startColor.a = 0f;
        spriteRenderer.color = startColor;

        Sequence spawnSequence = DOTween.Sequence();
        spawnSequence
            .Append(transform.DOScale(Vector3.one, 2f).SetEase(Ease.Linear))
            .Join(spriteRenderer.DOFade(1f, 2f))
            .AppendInterval(1f)
            .OnComplete(() =>
            {
                canMove = true;
            });
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
        if (collision.gameObject.CompareTag("LaserProjectile") || collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Paddle"))
        {
            // NEED EXPLOSION EFFECT HERE
            Destroy(gameObject);
        }

        PickNewDirection();
        directionTimer = directionChangeInterval;
    }
}

