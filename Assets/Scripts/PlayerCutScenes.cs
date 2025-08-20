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
    public UnityEvent<bool> canInteract;

    public TMP_Text textLabel;
    [SerializeField] float textLifetime = 3;
    [SerializeField] float fadeIn = 0.6f;
    [SerializeField] float fadeOut = 0.6f;

    public GameObject skipButton;

    CharacterController controller;
    PlayableDirector director;

    InputAction interactAction;
    GameObject trigger;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        director = GetComponent<PlayableDirector>();

        director.stopped += FinishCutscene;

        interactAction = GetComponent<PlayerInput>().actions["Interact"];
    }

    void Start() => StartCutscene("GameStart");

    void Update()
    {
        if (interactAction.triggered == false) return;

        if (trigger)
        {
            StartCutscene(trigger.name);
            return;
        }

        if (director.state == PlayState.Paused)
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

    void OnTriggerExit(Collider other) => canInteract?.Invoke(false);

    void StartCutscene(string triggerName)
    {
        TimelineAsset timeline = timelines.FirstOrDefault(timeline => timeline.name == triggerName);
        if (timeline == null) return;

        trigger = null;
        director.playableAsset = timeline;

        controller.enabled = false;
        HUD.SetActive(false);
        skipButton.SetActive(true);

        director.Play();
    }

    void FinishCutscene(PlayableDirector director = null)
    {
        controller.enabled = true;
        HUD.SetActive(true);
        skipButton.SetActive(false);

        Destroy(trigger);
    }

    public void WaitForInteract()
    {
        director.Pause();
        canInteract?.Invoke(true);
    }
    public void WaitForInteract2() => Debug.Log("h");

    public void WaitForText(string text) => StartCoroutine(RunText(text));

    IEnumerator RunText(string text)
    {
        textLabel.text = text;

        yield return FadeTo(1, fadeIn);
        yield return new WaitForSeconds(textLifetime);
        yield return FadeTo(0, fadeOut);
    }

    IEnumerator FadeTo(float target, float duration)
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
}
