using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public void ButtonSound() => AudioManager.AudioSource.PlayOneShot(AudioManager.Instance.buttonSound);
    public void NavigateSound() => AudioManager.AudioSource.PlayOneShot(AudioManager.Instance.navigateSound);
    public void SetTime(float timeScale) => Time.timeScale = timeScale;
    public void LoadScene(string sceneName) => SceneManager.Instance.LoadScene(sceneName);
    public void Quit() => Application.Quit();
}
