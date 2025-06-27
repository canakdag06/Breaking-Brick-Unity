using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Paddle : MonoBehaviour
{
    public Ball ball;
    public Transform ballLocation, bigBallLocation;
    public Transform leftWall, rightWall;
    public GameObject[] paddleVisuals;

    private InputReader inputReader;
    private Vector2 moveInput;


    [SerializeField] private int launchSpeed;
    [SerializeField] private float moveSpeed;
    private bool ballLaunched;

    private Transform activeVisual;
    private float halfWidth;
    private float halfWallWidth;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputReader = InputReader.Instance;
        ball.transform.parent = transform;
        ball.transform.position = ballLocation.position;

        BoxCollider2D col = leftWall.GetComponent<BoxCollider2D>();
        halfWallWidth = col.bounds.extents.x;

        halfWidth = GetCurrentPaddleHalfWidth();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = inputReader.MoveInput;
        //rb.MovePosition(rb.position + Vector2.right * input * speed * Time.deltaTime);

        //transform.Translate(moveSpeed * Time.deltaTime * moveInput);

        Vector3 pos = transform.position;
        pos.x += moveSpeed * Time.deltaTime * moveInput.x;

        if (inputReader.Throw && ball != null && !ballLaunched)
        {
            ball.transform.parent = null;
            ballLaunched = true;
            ball.Launch(new Vector2(1f, 1f), launchSpeed);
        }

        pos.x = Mathf.Clamp(pos.x, (leftWall.position.x + halfWallWidth) + halfWidth, (rightWall.position.x - halfWallWidth) - halfWidth);
        transform.position = pos;

        inputReader.ResetInputs();
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
