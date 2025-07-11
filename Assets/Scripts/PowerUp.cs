using UnityEngine;

public enum PowerUpType
{
    DuplicateBall,
    Expand,
    ExtraLife,
    FlamingBall,
    Laser,
    Magnet
}

[System.Serializable]
public class PowerUpData
{
    public PowerUpType type;
    public Sprite sprite;
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpData[] powerUpDatas;

    public PowerUpType type { get; private set; }
    public float fallSpeed = 2f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        int randIndex = Random.Range(0, powerUpDatas.Length);
        PowerUpData selectedData = powerUpDatas[randIndex];
        type = selectedData.type;

        sr.sprite = selectedData.sprite;
    }

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paddle"))
        {
            // Apply power-up
            Destroy(gameObject);
        }
        else if (other.CompareTag("DeathWall"))
        {
            Destroy(gameObject);
        }
    }
}
