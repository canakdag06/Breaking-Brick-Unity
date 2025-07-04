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
        for (int i = 0; i < lives.Length; i++)
        {
            if (lives[i] != null)
            {
                lives[i].gameObject.SetActive(i < currentLives - 1);
            }
        }
    }
}
