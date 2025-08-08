using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPanels : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;
    public GameObject settingsPanel;
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

        SceneManager.LoadScene("Game");
    }
}
