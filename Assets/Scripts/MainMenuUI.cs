using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Sliders")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider volumeSlider;

    private void Start() => ShowPanel(mainButtonsPanel, true);

    public void OpenSettings()
    {
        ShowPanel(settingsPanel, true);
        ShowPanel(mainButtonsPanel, false);

        var manager = Object.FindFirstObjectByType<SettingsManager>();
        manager?.AssignSliders(brightnessSlider, volumeSlider);
    }

    public void CloseSettings()
    {
        ShowPanel(settingsPanel, false);
        ShowPanel(mainButtonsPanel, true);
    }

    public void StartNewGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ShowPanel(GameObject panel, bool visible)
    {
        if (panel != null)
            panel.SetActive(visible);
    }
}
