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
    [SerializeField] private Animation lasersEnabledAnimation;
    [SerializeField] private GameObject laserProjectilePrefab;

    [Header("Movement Settings")]
    [SerializeField] private int launchSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float shootRate;

    [Header("Visuals")]
    [SerializeField] private PaddleVisualData[] visualDatas;
    private int visualLevel = 0;

    private InputReader inputReader;
    private Vector2 moveInput;

    private Coroutine shrinkRoutine;
    private bool isExpanded;

    private Ball ball;
    private bool ballLaunched;
    private float movementTimer = 0f;
    private int lastMoveDirection = 0; // -1 = left, 1 = right, 0 = not moving
    private const float maxInfluenceTime = 0.3f;

    private BoxCollider2D paddleCollider;
    private float halfWidth;
    private float halfWallWidth;

    private Coroutine disableLaserRoutine;
    private Coroutine shootingRoutine;
    private Coroutine disableMagnetRoutine;
    private bool isLaserActive;

    private bool isMagnetActive = false;
    private bool isMagnetLaunchReady = false;

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
            isMagnetLaunchReady = false;
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
    private void UpdatePaddleHalfWidth()
    {
        halfWidth = paddleRenderer.bounds.extents.x;
    }

    public void ExpandPaddle(float expandDuration)
    {
        //if (visualLevel >= visualDatas.Length - 1)
        //    return;

        if(isExpanded)
        {
            return;
        }

        StartCoroutine(PlayExpandAnimation());

        if (shrinkRoutine != null)
            StopCoroutine(shrinkRoutine);

        ShrinkAfterDelay(expandDuration);
    }

    private IEnumerator PlayExpandAnimation()
    {
        int targetIndex = visualDatas.Length - 1;
        isExpanded = true;

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

    public void ShrinkAfterDelay(float delay)
    {
        if (!isExpanded)
            return;

        if (shrinkRoutine != null)
        {
            StopCoroutine(shrinkRoutine);
            shrinkRoutine = null;
        }

        shrinkRoutine = StartCoroutine(ShrinkCoroutine(delay));
    }

    private IEnumerator ShrinkCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

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
        isExpanded = false;
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

    // ======================= EFFECT =======================
    public void PlayLifeLostEffect()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(paddleRenderer.DOColor(Color.red, 0.1f));
        seq.Append(transform.DOPunchPosition(new Vector3(0.1f, 0f, 0f), 0.3f, 30, 0.5f));
        seq.Append(paddleRenderer.DOColor(Color.white, 0.1f));
        seq.OnComplete(() => BallManager.Instance.SpawnInitialBall());
    }

    // ======================= POWER-UPS =======================

    public void EnableLaser(float laserDuration)
    {
        if (disableLaserRoutine != null && shootingRoutine != null)
        {
            StopCoroutine(disableLaserRoutine);
            StopCoroutine(shootingRoutine);
            disableLaserRoutine = null;
            shootingRoutine = null;
        }
        else
        {
            // Animation
            lasersEnabledAnimation[lasersEnabledAnimation.clip.name].speed = 1f;
            lasersEnabledAnimation[lasersEnabledAnimation.clip.name].time = 0f;
            leftLaser.gameObject.SetActive(true);
            rightLaser.gameObject.SetActive(true);
            lasersEnabledAnimation.Play();
        }

        disableLaserRoutine = StartCoroutine(LaserTimerCoroutine(laserDuration));
        shootingRoutine = StartCoroutine(Shoot());
        isLaserActive = true;
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootRate);
            Vector2 left = leftLaser.position;
            Vector2 right = rightLaser.position;
            Instantiate(laserProjectilePrefab, left, Quaternion.identity);
            Instantiate(laserProjectilePrefab, right, Quaternion.identity);
        }
    }

    public void LaserTimer(float laserDuration)
    {
        if (!isLaserActive)
            return;

        if (disableLaserRoutine != null)
        {
            StopCoroutine(disableLaserRoutine);
            disableLaserRoutine = null;
        }

        disableLaserRoutine = StartCoroutine(LaserTimerCoroutine(laserDuration));
    }


    private IEnumerator LaserTimerCoroutine(float laserDuration)
    {
        yield return new WaitForSeconds(laserDuration);

        // Animation
        lasersEnabledAnimation[lasersEnabledAnimation.clip.name].speed = -1f;
        lasersEnabledAnimation[lasersEnabledAnimation.clip.name].time = lasersEnabledAnimation[lasersEnabledAnimation.clip.name].length;
        lasersEnabledAnimation.Play();

        if (shootingRoutine != null)
        {
            StopCoroutine(shootingRoutine);
            shootingRoutine = null;
        }

        // Wait for animation to end
        yield return new WaitForSeconds(lasersEnabledAnimation[lasersEnabledAnimation.clip.name].length);

        leftLaser.gameObject.SetActive(false);
        rightLaser.gameObject.SetActive(false);
        disableLaserRoutine = null;
        isLaserActive = false;
    }

    public void EnableMagnet(float magnetDuration)
    {
        if (disableMagnetRoutine != null)
        {
            StopCoroutine(disableMagnetRoutine);
            disableMagnetRoutine = null;
        }

        isMagnetActive = true;
        disableMagnetRoutine = StartCoroutine(MagnetTimer(magnetDuration));
    }

    private IEnumerator MagnetTimer(float magnetDuration)
    {
        yield return new WaitForSeconds(magnetDuration);
        disableMagnetRoutine = null;
        isMagnetActive = false;
    }

    public void DisableMagnet()
    {
        if (disableMagnetRoutine != null)
        {
            StopCoroutine(disableMagnetRoutine);
            disableMagnetRoutine = null;
        }
        isMagnetActive = false;
        isMagnetLaunchReady = false;
    }

    public bool TryMagnetAttach(Ball ball)
    {
        if (!isMagnetActive || isMagnetLaunchReady || disableMagnetRoutine == null)
        {
            return false;
        }
        isMagnetLaunchReady = true;

        ball.Stop();
        ball.transform.SetParent(transform);
        ball.transform.position = ballLocation.position;

        SetBall(ball);

        return true;
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