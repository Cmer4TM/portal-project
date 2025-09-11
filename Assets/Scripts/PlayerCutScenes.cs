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

    [SerializeField] float playerTextTime = 3f;
    [SerializeField] float fadeIn = 0.6f;
    [SerializeField] float fadeOut = 0.6f;

    CharacterController controller;
    PlayableDirector director;
    GameObject trigger;
    Coroutine textCoroutine;
    string currentTimelineName;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        director = GetComponent<PlayableDirector>();
        director.stopped += FinishCutscene;
    }

    void Start() => StartCutscene("GameStart");

    public void OnInteract()
    {
        if (trigger)
        {
            var anim = trigger.GetComponentInParent<Animator>();
            if (anim) anim.SetTrigger(trigger.name);
        }

        if (director.state == PlayState.Paused && director.time != 0)
        {
            canInteract?.Invoke(false);
            director.Resume();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        trigger = other.gameObject;

        if (other.CompareTag("Interactable"))
        {
            canInteract?.Invoke(true);
            return;
        }

        StartCutscene(other.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == trigger) trigger = null;
        canInteract?.Invoke(false);
    }

    void StartCutscene(string triggerName)
    {
        var timeline = timelines.FirstOrDefault(t => t && t.name == triggerName);
        if (!timeline) return;

        currentTimelineName = triggerName;
        trigger = null;

        director.playableAsset = timeline;

        if (controller) controller.enabled = false;
        if (HUD) HUD.SetActive(false);
        if (skipButton) skipButton.SetActive(true);

        director.Play();
    }

    void FinishCutscene(PlayableDirector _)
    {
        if (controller) controller.enabled = true;
        if (HUD) HUD.SetActive(true);
        if (skipButton) skipButton.SetActive(false);

        if (trigger && !trigger.CompareTag("Interactable"))
        {
            var toDestroy = trigger;
            trigger = null;
            Destroy(toDestroy);
        }
    }

    public void WaitForText(string text)
    {
        if (textCoroutine != null) StopCoroutine(textCoroutine);
        textCoroutine = StartCoroutine(RunText(text));
    }

    IEnumerator RunText(string text)
    {
        if (!textLabel) yield break;

        textLabel.gameObject.SetActive(true);
        textLabel.alpha = 0f;
        textLabel.text = text;

        yield return FadeTMP(1f, fadeIn);
        yield return new WaitForSeconds(playerTextTime);
        yield return FadeTMP(0f, fadeOut);

        textCoroutine = null;
    }

    IEnumerator FadeTMP(float target, float duration)
    {
        float start = textLabel.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            textLabel.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        textLabel.alpha = target;
        if (Mathf.Approximately(target, 0f)) textLabel.gameObject.SetActive(false);
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
