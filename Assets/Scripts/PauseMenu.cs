using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    [Header("Sliders")]
    public Slider brightnessSlider;
    public Slider volumeSlider;

    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuPanel?.SetActive(isPaused);
        settingsPanel?.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        pauseMenuPanel?.SetActive(false);

        if (SettingsManager.Instance != null)
            SettingsManager.Instance.AssignSliders(brightnessSlider, volumeSlider);
    }

    public void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(true);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuUIScene");
    }

    public void ExitToDesktop()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
