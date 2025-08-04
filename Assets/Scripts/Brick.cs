using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public BrickColor color;
    public BrickData brickData;

    [SerializeField] private Collider2D brickCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int health = 1;
    [SerializeField] private bool isBreakable;
    [SerializeField] private float breakFrameDelay = 0.05f;

    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float dropChance = 0.2f;

    private Collider2D col;
    private Sprite[] damageSprites;
    private int hits = 0;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        BallManager.Instance.OnFlamingBallSwitch += SwitchColliderMode;

        var data = brickData.bricks.Find(b => b.color == color);

        if (data == null)
        {
            Debug.LogError($"No data found for color {color}");
            return;
        }

        damageSprites = data.damageSprites;
        isBreakable = data.isBreakable;
        spriteRenderer.sprite = data.defaultSprite;

        UpdateSprite();
    }

    public void Hit()
    {
        if (!isBreakable)
        {
            return;
        }

        hits++;
        health--;
        if (health <= 0)
        {
            brickCollider.enabled = false;
            StartCoroutine(PlayBreakAnimation());
            ScoreManager.Instance.AddScore(hits);
        }
        else
        {
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        if (damageSprites.Length == 0)
            return;

        spriteRenderer.sprite = damageSprites[hits];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            return;

        Hit();
    }

    private IEnumerator PlayBreakAnimation()
    {
        DropPowerUp();
        for (int i = hits; i < damageSprites.Length; ++i)
        {
            spriteRenderer.sprite = damageSprites[i];
            yield return new WaitForSeconds(breakFrameDelay);
        }
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        if (brickData == null) return;

        var data = brickData.bricks.Find(b => b.color == color);
        if (data != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = data.defaultSprite;
            }
        }
    }

    private void DropPowerUp()
    {
        if (Random.value <= dropChance)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDisable()
    {
        BallManager.Instance.OnFlamingBallSwitch -= SwitchColliderMode;
    }

    private void SwitchColliderMode(bool isEnable)
    {
        if (isEnable)
        {
            col.isTrigger = true;
        }
        else
        {
            col.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isBreakable)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Ball"))
        {
            brickCollider.enabled = false;
            StartCoroutine(PlayBreakAnimation());
            ScoreManager.Instance.AddScore(health);
        }
        else if (collision.gameObject.CompareTag("LaserProjectile"))
        {
            Hit();
            Destroy(collision.gameObject);
        }
    }
}
