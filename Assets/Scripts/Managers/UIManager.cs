using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Level UI")]
    [SerializeField] private TextMeshProUGUI levelInfoText;
    [SerializeField] private float blinkInterval;
    [SerializeField] private int blinkCount = 3;

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Lives UI")]
    [SerializeField] private Transform[] lives;

    [SerializeField] private Image fadeImage;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScoreText;
        GameManager.Instance.OnLifeChanged += UpdateLivesUI;

    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScoreText;
        GameManager.Instance.OnLifeChanged -= UpdateLivesUI;

        transform.DOKill();
    }

    public IEnumerator ShowMessage(string message)
    {
        levelInfoText.text = "LEVEL " + (LevelManager.Instance.CurrentLevelIndex + 1) + "\n"
                                      + message;

        for (int i = 0; i < blinkCount; i++)
        {
            levelInfoText.enabled = true;   // ON
            yield return new WaitForSeconds(blinkInterval);

            levelInfoText.enabled = false; // OFF
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        Color c = fadeImage.color;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            fadeImage.color = c;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        Color c = fadeImage.color;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            fadeImage.color = c;
            yield return null;
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
            scoreText.transform.DOKill();
            scoreText.transform.localScale = Vector3.one;
            scoreText.transform.DOScale(1.3f, 0.1f).SetLoops(2, LoopType.Yoyo);
        }
    }

    private void UpdateLivesUI(int currentLives)
    {
        for (int i = 0; i < lives.Length; i++)
        {
            GameObject lifeObj = lives[i].gameObject;
            Transform t = lifeObj.transform;
            UnityEngine.UI.Image image = lifeObj.GetComponent<UnityEngine.UI.Image>();

            bool shouldBeActive = i < currentLives - 1;

            // adding
            if (!lifeObj.activeSelf && shouldBeActive)
            {
                lifeObj.SetActive(true);
                t.localScale = Vector3.zero;
                t.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            }

            // subtracting
            else if (lifeObj.activeSelf && !shouldBeActive)
            {
                Color originalColor = image.color;
                image.color = Color.red;

                // being red + scale down → disable
                t.DOScale(0f, 0.5f).SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        lifeObj.SetActive(false);
                        t.localScale = Vector3.one;
                        image.color = originalColor;
                    });
            }
        }
    }
}
