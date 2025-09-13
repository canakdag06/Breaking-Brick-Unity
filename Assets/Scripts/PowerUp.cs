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

    private float pulseSpeed = 4f;
    private float pulseAmount = 0.05f;
    private Vector3 initialScale;

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

        initialScale = transform.localScale;
    }

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = initialScale * scaleFactor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paddle"))
        {
            PowerUpManager.Instance.ApplyPowerUp(type);
            Destroy(gameObject);
        }
        else if (other.CompareTag("DeathWall"))
        {
            Destroy(gameObject);
        }
    }
}
