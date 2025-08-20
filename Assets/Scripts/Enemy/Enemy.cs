using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float directionChangeInterval;
    public float noiseStrength;
    public float raycastCheckDistance;

    public LayerMask obstacleLayers;

    [Header("Effects")]
    [SerializeField] private Sprite[] enemyExplosionSprites;
    [SerializeField] private float explosionFrameDelay;

    public static event Action OnEnemyExplode;

    private Vector2 currentDirection;
    private float directionTimer;
    private Transform paddleTransform;
    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private bool canMove = false;

    private Sprite[] sprites;
    private float frameRate;
    private int currentFrame;
    private float animationTimer;
    private SpriteRenderer sr;
    private readonly float baseMoveSpeed = 0.5f;
    private readonly float baseFrameRate = 1f;

    private bool isDying = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        paddleTransform = GameObject.FindWithTag("Paddle").transform;

        SpawnEffect();

        PickNewDirection();
        directionTimer = directionChangeInterval;
    }

    public void Initialize(Sprite[] animationSprites)
    {
        enemyCollider.enabled = false;
        sprites = animationSprites;
        frameRate = baseFrameRate * (baseMoveSpeed / moveSpeed);
        currentFrame = 0;
        animationTimer = 0f;
    }

    void Update()
    {
        if(isDying)
        {
            return;
        }

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
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
            .OnComplete(() =>
            {
                enemyCollider.enabled = true;
                canMove = true;
            });
    }

    private IEnumerator ExplodeEnemy()
    {
        isDying = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        enemyCollider.enabled = false;
        for (int i = 0; i < enemyExplosionSprites.Length; i++)
        {
            spriteRenderer.sprite = enemyExplosionSprites[i];
            yield return new WaitForSeconds(explosionFrameDelay);
        }
        Destroy(gameObject);
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
            OnEnemyExplode.Invoke();
            StartCoroutine(ExplodeEnemy());
        }

        if(isDying)
        {
            return;
        }
        PickNewDirection();
        directionTimer = directionChangeInterval;
    }
}

