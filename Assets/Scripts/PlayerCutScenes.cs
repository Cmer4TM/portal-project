using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController), typeof(PlayableDirector), typeof(PlayerInput))]
public class PlayerCutScenes : MonoBehaviour
{
    public TimelineAsset[] timelines;
    public GameObject HUD;
    public UnityEvent<bool> canInteract;

    [Header("Text UI")]
    [SerializeField] CanvasGroup textScreen;
    [SerializeField] TMP_Text textLabel;
    [SerializeField] float textLifetime = 3f;
    [SerializeField] float fadeIn = 0.6f;
    [SerializeField] float fadeOut = 0.6f;

    [Header("Skip UI")]
    [SerializeField] Button skipButton;
    [SerializeField] float skipVisibleSeconds = 5f;
    [SerializeField] int skipSortingOrder = 1200;

    PlayerInput playerInput;
    CharacterController controller;
    PlayableDirector director;

    InputAction interactAction;
    GameObject trigger;
    Coroutine textRoutine, skipRoutine;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        director = GetComponent<PlayableDirector>();
        director.stopped += FinishCutscene;
        interactAction = playerInput.actions["Interact"];

        if (textScreen)
        {
            textScreen.alpha = 0f;
            textScreen.interactable = textScreen.blocksRaycasts = false;
            textScreen.gameObject.SetActive(false);
        }
        if (skipButton)
        {
            skipButton.onClick.AddListener(SkipCutscene);
            skipButton.gameObject.SetActive(false);
        }
    }

    void Start() => StartCutscene("GameStart");

    void Update()
    {
        if (!interactAction.triggered) return;

        if (trigger) { StartCutscene(trigger.name); return; }

        if (director.state == PlayState.Paused)
        {
            canInteract?.Invoke(false);
            director.Resume();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        trigger = other.gameObject;
        if (other.CompareTag("Interactable")) { canInteract?.Invoke(true); return; }
        StartCutscene(other.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == trigger) trigger = null;
        canInteract?.Invoke(false);
    }

    void StartCutscene(string key)
    {
        var tl = timelines.FirstOrDefault(t => t && t.name == key);
        if (!tl) return;

        trigger = null;
        director.playableAsset = tl;

        controller.enabled = false;
        if (HUD) HUD.SetActive(false);

        director.Play();
        ShowSkipFor(skipVisibleSeconds);
    }

    void FinishCutscene(PlayableDirector _)
    {
        controller.enabled = true;
        if (HUD) HUD.SetActive(true);
        canInteract?.Invoke(false);
        HideText();
        HideSkip();
        if (trigger) Destroy(trigger);
    }

    public void WaitForInteract()
    {
        if (director.state != PlayState.Playing) return;
        director.Pause();
        canInteract?.Invoke(true);
    }

    public void WaitForText(string text)
    {
        if (!textScreen || !textLabel) return;
        if (textRoutine != null) StopCoroutine(textRoutine);
        textRoutine = StartCoroutine(RunText(text));
    }

    IEnumerator RunText(string s)
    {
        textLabel.text = s;
        textScreen.alpha = 0f;
        textScreen.gameObject.SetActive(true);
        textScreen.interactable = textScreen.blocksRaycasts = false;

        yield return FadeTo(1f, Mathf.Max(0.05f, fadeIn));
        yield return new WaitForSecondsRealtime(textLifetime);
        yield return FadeTo(0f, Mathf.Max(0.05f, fadeOut));

        textScreen.gameObject.SetActive(false);
        textRoutine = null;
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = textScreen.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            textScreen.alpha = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        textScreen.alpha = target;
    }

    void ShowSkipFor(float seconds)
    {
        if (!skipButton) return;
        var go = skipButton.gameObject;
        go.SetActive(true);
        var cv = go.GetComponent<Canvas>() ?? go.gameObject.AddComponent<Canvas>();
        cv.overrideSorting = true;
        cv.sortingOrder = skipSortingOrder;
        if (!go.GetComponent<GraphicRaycaster>()) go.AddComponent<GraphicRaycaster>();
        if (skipRoutine != null) StopCoroutine(skipRoutine);
        skipRoutine = StartCoroutine(HideSkipAfter(seconds));
    }

    IEnumerator HideSkipAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        HideSkip();
        skipRoutine = null;
    }

    void HideSkip()
    {
        if (skipButton) skipButton.gameObject.SetActive(false);
    }

    void HideText()
    {
        if (!textScreen) return;
        textScreen.alpha = 0f;
        textScreen.interactable = textScreen.blocksRaycasts = false;
        textScreen.gameObject.SetActive(false);
    }

    void SkipCutscene()
    {
        HideSkip();
        if (skipRoutine != null) { StopCoroutine(skipRoutine); skipRoutine = null; }

        double end = director.playableAsset is TimelineAsset ta ? ta.duration : director.duration;
        if (end <= 0) end = 0.0001f;
        director.time = end;
        director.Evaluate();
        director.Pause();

        FinishCutscene(director);
    }
}
