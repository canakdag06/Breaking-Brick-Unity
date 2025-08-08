using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanels : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;
    public GameObject settingsPanel;

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
        SceneManager.LoadScene("Game");
    }
}
