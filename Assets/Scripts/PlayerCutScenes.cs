using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController), typeof(PlayableDirector), typeof(PlayerInput))]
public class PlayerCutScenes : MonoBehaviour
{
    public TimelineAsset[] timelines;
    public GameObject HUD;
    public TMP_Text textLabel;
    public GameObject skipButton;
    public UnityEvent<bool> canInteract;
    
    [SerializeField] float playerTextTime;
    [SerializeField] float fadeIn;
    [SerializeField] float fadeOut;

    CharacterController controller;
    PlayableDirector director;
    GameObject trigger;
    Coroutine textCoroutine;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        director = GetComponent<PlayableDirector>();

        director.stopped += FinishCutscene;
    }

    void Start() => StartCutscene("GameStart");

    public void OnInteract()
    {
        if (trigger) trigger.GetComponentInParent<Animator>().SetTrigger(trigger.name);

        if (director.state == PlayState.Paused && director.time != 0)
        {
            canInteract?.Invoke(false);
            director.Resume();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        trigger = other.gameObject;

        switch (other.tag)
        {
            case "Interactable":
                canInteract?.Invoke(true);

                break;

            case "Cutscene":
                StartCutscene(other.name);

                break;
        }
    }

    void OnTriggerExit(Collider other) => canInteract?.Invoke(false);

    void StartCutscene(string triggerName)
    {
        if (timelines.FirstOrDefault(timeline => timeline.name == triggerName) is TimelineAsset timeline)
        {
            Destroy(trigger);
            director.playableAsset = timeline;

            controller.enabled = false;
            HUD.SetActive(false);
            skipButton.SetActive(true);

            director.Play();
        }
    }

    void FinishCutscene(PlayableDirector director)
    {
        controller.enabled = true;
        HUD.SetActive(true);
        skipButton.SetActive(false);
    }

    public void WaitForText(string text)
    {
        if (textCoroutine != null) StopCoroutine(textCoroutine);

        textCoroutine = StartCoroutine(RunText(text));
    }

    IEnumerator RunText(string text)
    {
        textLabel.alpha = 0;
        textLabel.text = text;

        yield return FadeTMP(1, fadeIn);
        yield return new WaitForSeconds(playerTextTime);
        yield return FadeTMP(0, fadeOut);

        textCoroutine = null;
    }

    IEnumerator FadeTMP(float target, float duration)
    {
        float start = textLabel.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            textLabel.alpha = Mathf.Lerp(start, target, time / duration);

            yield return null;
        }
    }

    public void SkipCutscene()
    {
        canInteract?.Invoke(false);

        director.Pause();
        director.time = director.duration;
        director.Resume();
    }

    public void WaitForInteract()
    {
        director.Pause();
        canInteract?.Invoke(true);
    }
}
