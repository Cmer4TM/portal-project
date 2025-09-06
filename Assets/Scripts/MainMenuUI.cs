using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void LoadScene(string sceneName) => SceneManager.Instance.LoadScene(sceneName);
    public void ExitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
