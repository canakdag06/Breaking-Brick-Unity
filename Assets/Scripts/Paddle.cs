using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class Paddle : MonoBehaviour
{
    public static Paddle Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform ballLocation;
    [SerializeField] private Transform leftWall, rightWall;
    [SerializeField] private SpriteRenderer paddleRenderer;
    [SerializeField] private Transform leftLaser, rightLaser;

    [Header("Movement Settings")]
    [SerializeField] private int launchSpeed;
    [SerializeField] private float moveSpeed;

    [Header("Visuals")]
    [SerializeField] private PaddleVisualData[] visualDatas;
    private int visualLevel = 0;

    private InputReader inputReader;
    private Vector2 moveInput;

    [SerializeField] private float expandDuration = 20f;
    private Coroutine shrinkRoutine;

    private Ball ball;
    private bool ballLaunched;
    private float movementTimer = 0f;
    private int lastMoveDirection = 0; // -1 = left, 1 = right, 0 = not moving
    private const float maxInfluenceTime = 0.3f;

    private BoxCollider2D paddleCollider;
    private float halfWidth;
    private float halfWallWidth;


    // ======================= LIFECYCLE =======================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        paddleCollider = GetComponent<BoxCollider2D>();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLifeLostEffectRequested -= PlayLifeLostEffect;
        }
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLifeLostEffectRequested += PlayLifeLostEffect;
        }
        inputReader = InputReader.Instance;

        BoxCollider2D col = leftWall.GetComponent<BoxCollider2D>();
        halfWallWidth = col.bounds.extents.x;
        UpdatePaddleHalfWidth();
    }

    // Update is called once per frame
    void Update()
    {
        PaddleMovement();
        ControlBall();
        inputReader.ResetInputs();
    }



    // ======================= BALL CONTROL =======================
    public void SetBall(Ball newBall)
    {
        ball = newBall;
        ball.transform.SetParent(transform);
        ball.transform.position = ballLocation.position;
        ballLaunched = false;
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

    // ======================= MOVEMENT =======================
    private void PaddleMovement()
    {
        moveInput = inputReader.MoveInput;
        Vector3 pos = transform.position;
        pos.x += moveSpeed * Time.deltaTime * moveInput.x;
        pos.x = Mathf.Clamp(pos.x, (leftWall.position.x + halfWallWidth) + halfWidth, (rightWall.position.x - halfWallWidth) - halfWidth);
        transform.position = pos;
    }

    // ======================= VISUAL CONTROL =======================

    private void ApplyVisual(int level)
    {
        if (level < 0 || level > visualDatas.Length)
        {
            Debug.LogWarning($"Invalid paddle visual level: {level}");
        }
        visualLevel = level;

        var data = visualDatas[level];
        paddleRenderer.sprite = data.sprite;
        leftLaser.localPosition = data.leftLaserPosition;
        rightLaser.localPosition = data.rightLaserPosition;
        paddleCollider.size = data.colliderSize;
        paddleCollider.offset = data.colliderOffset;

        UpdatePaddleHalfWidth();
    }


    private void UpdatePaddleHalfWidth()
    {
        halfWidth = paddleRenderer.bounds.extents.x;
    }

    public void ExpandPaddle()
    {
        if (visualLevel >= visualDatas.Length - 1)
            return;

        StopAllCoroutines();
        StartCoroutine(PlayExpandAnimation());

        if (shrinkRoutine != null)
            StopCoroutine(shrinkRoutine);

        shrinkRoutine = StartCoroutine(ShrinkAfterDelay());
    }

    private IEnumerator PlayExpandAnimation()
    {
        int targetIndex = visualDatas.Length - 1;

        for (int i = visualLevel + 1; i <= targetIndex; i++)
        {
            paddleRenderer.sprite = visualDatas[i].sprite;
            UpdateLaserPositions(i);
            UpdateColliderSize(i);
            UpdatePaddleHalfWidth();
            yield return new WaitForSeconds(0.1f);
        }

        visualLevel = targetIndex;
    }

    private IEnumerator ShrinkAfterDelay()
    {
        yield return new WaitForSeconds(expandDuration);

        for (int i = visualLevel - 1; i >= 0; i--)
        {
            paddleRenderer.sprite = visualDatas[i].sprite;
            UpdateLaserPositions(i);
            UpdateColliderSize(i);
            UpdatePaddleHalfWidth();
            yield return new WaitForSeconds(0.1f);
        }

        visualLevel = 0;
        shrinkRoutine = null;
    }

    private void UpdateLaserPositions(int index)
    {
        leftLaser.localPosition = visualDatas[index].leftLaserPosition;
        rightLaser.localPosition = visualDatas[index].rightLaserPosition;
    }

    private void UpdateColliderSize(int index)
    {
        paddleCollider.size = visualDatas[index].colliderSize;
        paddleCollider.offset = visualDatas[index].colliderOffset;
    }

    //public void ShrinkPaddle()
    //{
    //    int nextLevel = Mathf.Max(visualLevel - 1, 0);
    //    ApplyVisual(nextLevel);
    //}

    // ======================= EFFECT =======================
    public void PlayLifeLostEffect()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(paddleRenderer.DOColor(Color.red, 0.1f));
        seq.Append(transform.DOPunchPosition(new Vector3(0.1f, 0f, 0f), 0.3f, 30, 0.5f));
        seq.Append(paddleRenderer.DOColor(Color.white, 0.1f));
        seq.OnComplete(() => BallManager.Instance.SpawnInitialBall());
    }

    // ======================= ENABLE LASER =======================
    
    public void EnableLaser()
    {
        return;
    }

    public void EnableMagnet()
    {
        return;
    }
}


[System.Serializable]
public class PaddleVisualData
{
    public Sprite sprite;
    public Vector2 leftLaserPosition;
    public Vector2 rightLaserPosition;
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
}