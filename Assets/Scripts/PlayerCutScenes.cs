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
    
    [SerializeField] float playerTextTime = 5;
    [SerializeField] float fadeIn = 0.6f;
    [SerializeField] float fadeOut = 0.6f;

    CharacterController controller;
    PlayableDirector director;
    InputAction interactAction;
    GameObject trigger;
    Coroutine textCoroutine;

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
        if (interactAction.triggered)
        {
            if (trigger) trigger.transform.parent.GetComponent<Animator>().SetTrigger(trigger.name);

            if (director.state == PlayState.Paused)
            {
                canInteract?.Invoke(false);
                director.Resume();
            }
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

    void FinishCutscene(PlayableDirector director)
    {
        controller.enabled = true;
        HUD.SetActive(true);
        skipButton.SetActive(false);

        Destroy(trigger);
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

        yield return FadeTMP(textLabel, 1, fadeIn);
        yield return new WaitForSeconds(playerTextTime);
        yield return FadeTMP(textLabel, 0, fadeOut);

        textCoroutine = null;
    }

    static public IEnumerator FadeTMP(TMP_Text tmp, float target, float duration)
    {
        float start = tmp.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            tmp.alpha = Mathf.Lerp(start, target, time / duration);

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
