using UnityEngine;

public class Brick : MonoBehaviour
{
    public BrickColor color;
    public BrickData brickData;

    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private int health = 1;
    [SerializeField] private bool isBreakable;
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
        renderer.sprite = data.defaultSprite;

        //hitPoints = isBreakable ? -1 : damageSprites.Length;
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
            Destroy(gameObject);
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
        
        renderer.sprite = damageSprites[hits];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Hit();
    }

    private void OnValidate()
    {
        if (brickData == null) return;

        var data = brickData.bricks.Find(b => b.color == color);
        if (data != null)
        {
            renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = data.defaultSprite;
            }
        }
    }
}
