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
    [SerializeField] float fadeTime;

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
        if (trigger)
        {
            if (trigger.CompareTag("Portal Gun"))
            {
                Destroy(trigger);
                canInteract?.Invoke(false);

                trigger.transform.parent.GetComponent<Machine>().PortalGun();

                return;
            }

            Transform parent = trigger.transform.parent;

            if (parent.TryGetComponent(out Animator animator)) animator.SetTrigger(trigger.name);
            
            else if (parent.TryGetComponent(out Machine machine))
            {
                machine.Fix();
                canInteract?.Invoke(false);
            }
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

        switch (other.tag)
        {
            case "Interactable":
            case "Portal Gun":
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

        yield return GameManager.TextFade(textLabel, 1, fadeTime);
        yield return new WaitForSeconds(playerTextTime);
        yield return GameManager.TextFade(textLabel, 0, fadeTime);

        textCoroutine = null;
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
