using Unity.VisualScripting;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public static Paddle Instance { get; private set; }

    //public Ball ball;
    public Transform ballLocation/*, bigBallLocation*/;
    public Transform leftWall, rightWall;
    public GameObject[] paddleVisuals;

    private InputReader inputReader;
    private Vector2 moveInput;

    private Ball ball;
    [SerializeField] private int launchSpeed;
    [SerializeField] private float moveSpeed;
    private bool ballLaunched;
    private float movementTimer = 0f;
    private int lastMoveDirection = 0; // -1 = sol, 1 = sağ, 0 = sabit
    private const float maxInfluenceTime = 0.3f;

    private Transform activeVisual;
    private float halfWidth;
    private float halfWallWidth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        inputReader = InputReader.Instance;

        BoxCollider2D col = leftWall.GetComponent<BoxCollider2D>();
        halfWallWidth = col.bounds.extents.x;
        halfWidth = GetCurrentPaddleHalfWidth();
    }

    // Update is called once per frame
    void Update()
    {
        PaddleMovement();
        ControlBall();
        inputReader.ResetInputs();
    }

    public void SetBall(Ball newBall)
    {
        ball = newBall;
        ball.transform.SetParent(transform);
        ball.transform.position = ballLocation.position;
        ballLaunched = false;
    }

    private void PaddleMovement()
    {
        moveInput = inputReader.MoveInput;
        Vector3 pos = transform.position;
        pos.x += moveSpeed * Time.deltaTime * moveInput.x;
        pos.x = Mathf.Clamp(pos.x, (leftWall.position.x + halfWallWidth) + halfWidth, (rightWall.position.x - halfWallWidth) - halfWidth);
        transform.position = pos;
    }

    private void ControlBall()
    {
        if (moveInput.x != 0)
        {
            if (Mathf.Sign(moveInput.x) != lastMoveDirection)
            {
                movementTimer = 0f;
                lastMoveDirection = (int)Mathf.Sign(moveInput.x);
            }

            movementTimer += Time.deltaTime;
        }
        else
        {
            movementTimer = 0f;
            lastMoveDirection = 0;
        }


        if (inputReader.Throw && ball != null && !ballLaunched)
        {
            ball.transform.parent = BallManager.Instance.transform;
            ballLaunched = true;

            float influence = Mathf.Clamp01(movementTimer / maxInfluenceTime);
            float xDirection = lastMoveDirection * influence;

            Vector2 dir = new Vector2(xDirection, 1f).normalized;
            ball.Launch(dir, launchSpeed);
        }
    }

    float GetCurrentPaddleHalfWidth()
    {
        foreach (GameObject paddle in paddleVisuals)
        {
            if (paddle.activeSelf)
            {
                activeVisual = paddle.transform;
            }
        }

        SpriteRenderer[] renderers = activeVisual.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0) return 0f;

        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds.extents.x;
    }
}
