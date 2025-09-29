using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    public Animator door;
    public Image blackScreen;

    [SerializeField] bool inactive;
    [SerializeField] float fadeTime;
    [SerializeField] string nextSceneName;

    void Start()
    {
        if (inactive) door.SetBool("Unlocked", true);
    }

    public void ElevatorActivated()
    {
        door.SetBool("Unlocked", false);

        StartCoroutine(ScreenFadeOut());
    }

    IEnumerator ScreenFadeOut()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            blackScreen.color = Color.black * Mathf.Lerp(0, 1, elapsedTime / fadeTime);
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        blackScreen.color = Color.black;

        SceneManager.Instance.LoadScene(nextSceneName);
    }
}
