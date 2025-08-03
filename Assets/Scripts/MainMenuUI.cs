// MainMenuUI.cs
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainButtonsPanel;
    public GameObject settingsPanel;

    [Header("Sliders")]
    public Slider brightnessSlider;
    public Slider volumeSlider;

    private void Start()
    {
        ShowMainButtons();

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.AssignSliders(brightnessSlider, volumeSlider);
        }
    }

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        mainButtonsPanel?.SetActive(false);

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.AssignSliders(brightnessSlider, volumeSlider);
        }
    }

    public void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        mainButtonsPanel?.SetActive(true);
    }

    public void StartNewGame(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ShowMainButtons()
    {
        mainButtonsPanel?.SetActive(true);
        settingsPanel?.SetActive(false);
    }
}
