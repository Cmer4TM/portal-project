using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController), typeof(PlayableDirector))]
public class PlayerCutScenes : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController controller;
    private PlayableDirector director;

    public TimelineAsset[] timelines;
    public GameObject HUD;
    public UnityEvent<bool> canInteract;

    private InputAction interactAction;
    private GameObject trigger;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        director = GetComponent<PlayableDirector>();

        director.stopped += FinishCutscene;

        interactAction = playerInput.actions["Interact"];
    }

    private void Start() => StartCutscene("GameStart");

    private void Update()
    {
        if (interactAction.triggered)
        {
            if (trigger)
            {
                StartCutscene(trigger.name);
            }

            if (director.state == PlayState.Paused)
            {
                canInteract?.Invoke(false);
                director.Resume();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        trigger = other.gameObject;

        if (other.CompareTag("Interactable"))
        {
            canInteract?.Invoke(true);
            return;
        }

        StartCutscene(other.name);
    }

    private void OnTriggerExit(Collider other) => canInteract?.Invoke(false);

    private void StartCutscene(string triggerName)
    {
        if (timelines.First(timeline => timeline.name == triggerName) is TimelineAsset timeline)
        {
            trigger = null;

            director.playableAsset = timeline;

            controller.enabled = false;
            HUD.SetActive(false);

            director.Play();
        }
    }

    private void FinishCutscene(PlayableDirector director)
    {
        controller.enabled = true;
        HUD.SetActive(true);

        Destroy(trigger);
    }

    public void WaitForInteract()
    {
        if (director.state != PlayState.Playing)
        {
            return;
        }

        director.Pause();
        canInteract?.Invoke(true);
    }

    public void WaitForText(string text)
    {
        if (director.state != PlayState.Playing)
        {
            return;
        }

        director.Pause();

        // Опишу потом
    }
}
