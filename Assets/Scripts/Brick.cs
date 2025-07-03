using UnityEngine;

public class Brick : MonoBehaviour
{
    public BrickData data;
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private SpriteRenderer renderer;


    private void Start()
    {
        if (data == null)
        {
            Debug.Log("BrickData not assigned", this);
            return;
        }

        renderer.color = data.brickColor;
        UpdateSprite();
    }

    public void Hit()
    {
        if(!data.isBreakable)
        {
            return;
        }

        hitPoints--;
        if(hitPoints <= 0)
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
        if (data.damageSprites.Length == 0)
            return;

        int spriteIndex = Mathf.Clamp(data.damageSprites.Length - hitPoints, 0, data.damageSprites.Length - 1);
        renderer.sprite = data.damageSprites[spriteIndex];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
