using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public void TogglePause(bool active)
    {
        Time.timeScale = active ? 0 : 1;
        pauseMenuPanel.SetActive(active);
    }

    public void ExitToMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitToDesktop()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
