using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPanels : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;
    public GameObject settingsPanel;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform levelsContainer;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;


    public void OpenMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelsPanel.SetActive(false);
        //settingsPanel.SetActive(false);
    }

    public void OpenLevelsMenu()
    {
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(true);
        //settingsPanel.SetActive(false);

        UpdateLevelButtons();

    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(false);
        //settingsPanel.SetActive(true);
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

        SceneManager.LoadScene("Game");
    }

    private void UpdateLevelButtons()
    {
        int lastLevel = GameManager.Instance.LastPlayedLevel;

        for (int i = 0; i <= lastLevel; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelsContainer);
            buttonObj.name = "LevelButton_" + i;


            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = (i + 1).ToString();

            Button btn = buttonObj.GetComponent<Button>();
            int levelIndex = i;
            btn.onClick.AddListener(() => OnLevelSelected(levelIndex));
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        Debug.Log("Seçilen Level: " + levelIndex);

        GameManager.Instance.SetLastPlayedLevel(levelIndex);
        SceneManager.LoadScene("Game");
    }
}
