using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;    // Меню паузи
    public GameObject settingsPanel;     // Панель налаштувань

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuUIScene"); // заміни на назву своєї сцени
    }
}
