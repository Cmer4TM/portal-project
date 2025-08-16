using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject HUD;

    public void TogglePause(bool active)
    {
        Time.timeScale = active ? 0 : 1;
        pauseMenuPanel.SetActive(active);
        HUD.SetActive(active == false);
    }

    public void LoadScene(string sceneName) => SceneManager.Instance.LoadScene(sceneName);

    public void ExitToDesktop()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
