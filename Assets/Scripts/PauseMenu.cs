using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public void TogglePause(bool active)
    {
        Time.timeScale = active ? 0 : 1;
        pauseMenuPanel.SetActive(active);
    }

    public void ExitToDesktop()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
