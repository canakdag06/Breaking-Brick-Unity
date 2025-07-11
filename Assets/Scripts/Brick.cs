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

    private Sprite[] damageSprites;
    private int hits = 0;

    private void Start()
    {
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
}
