using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public GameObject canvas;
    public Slider slider;
    public TMP_Text progress;

    void Awake() => Instance = this;

    public void LoadScene(string sceneName)
    {
        canvas.SetActive(true);

        AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        StartCoroutine(LoadingUpdate(loading));
    }

    private IEnumerator LoadingUpdate(AsyncOperation loading)
    {
        while (loading.isDone == false)
        {
            slider.value = loading.progress;
            progress.text = $"{loading.progress * 100:0}%";

            yield return null;
        }

        canvas.SetActive(false);
    }
}
