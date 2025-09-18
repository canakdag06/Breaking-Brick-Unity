using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanels : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;
    public LevelInfoSO levelData;
    public GameObject settingsPanel;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform levelsContainer;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;


    public void OpenMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OpenLevelsMenu()
    {
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(true);
        settingsPanel.SetActive(false);

        UpdateLevelButtons();
    }

    public void CloseLevelsMenu()
    {
        levelsPanel.SetActive(false);
        ClearLevelButtons();
        mainMenuPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void PlayGame()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        GameManager.Instance.StartGame();
    }

    private void UpdateLevelButtons()
    {
        int lastLevel = GameManager.Instance.HighestLevelReached;

        for (int i = 0; i <= lastLevel; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelsContainer);
            buttonObj.name = "LevelButton_" + (i + 1);


            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = (i + 1).ToString();

            Image levelImg = buttonObj.GetComponentsInChildren<Image>()
                          .FirstOrDefault(img => img.gameObject != buttonObj);
            levelImg.sprite = levelData.levels[i].previewImage;

            Button btn = buttonObj.GetComponent<Button>();
            int levelIndex = i;
            btn.onClick.AddListener(() => OnLevelSelected(levelIndex));
        }
    }

    private void ClearLevelButtons()
    {
        foreach (Transform child in levelsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        GameManager.Instance.SetLastPlayedLevel(levelIndex);

        GameManager.Instance.ResetProgressForLevelSelection();

        GameManager.Instance.StartGame();
    }
}
