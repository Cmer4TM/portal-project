using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static IEnumerator TextFade(TMP_Text text, float target, float time)
    {
        float elapsedTime = 0;
        float startingAlpha = text.alpha;

        while (elapsedTime < time)
        {
            text.alpha = Mathf.Lerp(startingAlpha, target, elapsedTime / time);
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        text.alpha = target;
    }
}
