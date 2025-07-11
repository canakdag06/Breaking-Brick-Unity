using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform[] lives;

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScoreText;
        GameManager.Instance.OnLifeChanged += UpdateLivesUI;

    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScoreText;
        GameManager.Instance.OnLifeChanged -= UpdateLivesUI;
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
        //for (int i = 0; i < lives.Length; i++)
        //{
        //    if (lives[i] != null)
        //    {
        //        lives[i].gameObject.SetActive(i < currentLives - 1);
        //    }
        //}

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
