using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 5f;
    public Transform ballLocation, bigBallLocation;
    private Rigidbody2D rb;
    private bool inPlay = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!inPlay)
        {
            transform.position = ballLocation.position;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                inPlay = true;
                rb.linearVelocity = new Vector2(1f, 1f).normalized * speed;
            }
        }
    }
}
