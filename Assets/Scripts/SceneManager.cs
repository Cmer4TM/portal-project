using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    [SerializeField] private GameObject canvas;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text progress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
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
    }
}
