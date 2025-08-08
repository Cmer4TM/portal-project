using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayerInput), typeof(PlayableDirector))]
public class PlayerCutScenes : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayableDirector director;

    public TimelineAsset[] timelines;
    public GameObject HUD;
    public UnityEvent<bool> canInteract;

    private InputAction interactAction;
    private string triggerName;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        director = GetComponent<PlayableDirector>();

        director.stopped += FinishCutscene;

        interactAction = playerInput.actions["Interact"];

        StartCutscene("GameStart");
    }

    private void Update()
    {
        if (interactAction.triggered && triggerName != null)
        {
            StartCutscene(triggerName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            triggerName = other.name;
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
            this.triggerName = null;

            director.playableAsset = timeline;

            playerInput.enabled = false;
            HUD.SetActive(false);

            director.Play();
        }
    }

    private void FinishCutscene(PlayableDirector director)
    {
        playerInput.enabled = true;
        HUD.SetActive(true);

        director.gameObject.SetActive(false);
    }
}
