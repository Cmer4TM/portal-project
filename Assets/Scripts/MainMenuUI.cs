using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    void Awake() => Application.targetFrameRate = 120;
    public void LoadScene(string sceneName) => SceneManager.Instance.LoadScene(sceneName);
    public void ExitGame() => Application.Quit();
}
