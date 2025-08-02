using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainButtonsPanel;
    public GameObject settingsPanel;

    public void Start()
    {
        ShowMainButtons();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(true);
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
        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
