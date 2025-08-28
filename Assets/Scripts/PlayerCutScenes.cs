using System.Collections;
using System.Collections.Generic;
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
    public TMP_Text missionText;
    public GameObject skipButton;
    [SerializeField] GameObject missionIcon;
    [SerializeField] GameObject interactButton;
    public UnityEvent<bool> canInteract;
    public MissionData[] firstCutsceneMissions;
    [SerializeField] float missionTextShowSeconds = 5f;
    [SerializeField] float fadeIn = 0.6f, fadeOut = 0.6f;

    struct Mission { public string id; public string text; public bool showIconNow; }
    readonly Queue<Mission> missionQueue = new();

    string currentMissionId, currentMissionText, currentTimelineName;
    bool hasActiveMission;
    float lookSum;

    CharacterController controller;
    PlayableDirector director;
    InputAction interactAction;
    GameObject trigger;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        director = GetComponent<PlayableDirector>();
        director.stopped += FinishCutscene;
        interactAction = GetComponent<PlayerInput>()?.actions["Interact"];
        if (missionIcon) missionIcon.SetActive(false);
        if (interactButton) interactButton.SetActive(false);
        if (missionText)
        {
            missionText.overflowMode = TextOverflowModes.Overflow;
            missionText.alpha = 0f;
            missionText.gameObject.SetActive(false);
        }
    }

    void Start() => StartCutscene("GameStart");

    void Update()
    {
        if (interactAction != null && interactAction.triggered)
        {
            if (trigger) { StartCutscene(trigger.name); return; }
            if (director.state == PlayState.Paused)
            {
                if (interactButton) interactButton.SetActive(false);
                canInteract?.Invoke(false);
                director.Resume();
            }
        }

        if (hasActiveMission && currentMissionId == "look")
        {
            var look = GetComponent<PlayerInput>().actions["Look"];
            Vector2 d = look.ReadValue<Vector2>();
            lookSum += Mathf.Abs(d.x) + Mathf.Abs(d.y);
            if (lookSum >= 400f) CompleteMission("look");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        trigger = other.gameObject;
        StartCutscene(other.name);
    }

    void StartCutscene(string triggerName)
    {
        var t = timelines.FirstOrDefault(x => x.name == triggerName);
        if (!t) return;
        currentTimelineName = triggerName;
        trigger = null;
        director.playableAsset = t;
        controller.enabled = false;
        if (HUD) HUD.SetActive(false);
        if (skipButton) skipButton.SetActive(true);
        director.Play();
    }

    void FinishCutscene(PlayableDirector _ = null)
    {
        controller.enabled = true;
        if (HUD) HUD.SetActive(true);
        if (skipButton) skipButton.SetActive(false);
        if (interactButton) interactButton.SetActive(false);

        if (currentTimelineName == "GameStart" && firstCutsceneMissions != null)
            foreach (var md in firstCutsceneMissions)
                if (md) AddMission(md.id, md.text, md.showIconNow);

        if (trigger) Destroy(trigger);
    }

    public void WaitForText(string text) => StartCoroutine(RunText(text));

    IEnumerator RunText(string text)
    {
        if (!textLabel) yield break;
        textLabel.gameObject.SetActive(true);
        textLabel.text = text;
        yield return FadeTMP(textLabel, 1f, fadeIn);
        yield return new WaitForSeconds(3f);
        yield return FadeTMP(textLabel, 0f, fadeOut);
    }

    IEnumerator FadeTMP(TMP_Text tmp, float target, float duration)
    {
        float start = tmp.alpha, t = 0f;
        if (!tmp.gameObject.activeSelf) tmp.gameObject.SetActive(true);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            tmp.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        tmp.alpha = target;
        if (Mathf.Approximately(target, 0f)) tmp.gameObject.SetActive(false);
    }

    public void SkipCutscene()
    {
        director.Pause();
        director.time = director.duration;
        director.Resume();
    }

    public void WaitForInteract()
    {
        director.Pause();
        if (interactButton)
        {
            interactButton.SetActive(true);
            interactButton.transform.SetAsLastSibling();
        }
        canInteract?.Invoke(true);
    }

    public void AddMission(string id, string text, bool showIconNow = true)
    {
        missionQueue.Enqueue(new Mission { id = id, text = text, showIconNow = showIconNow });
        if (!hasActiveMission) StartNextMission();
    }

    void StartNextMission()
    {
        if (missionQueue.Count == 0) { hasActiveMission = false; return; }

        var m = missionQueue.Dequeue();
        currentMissionId = m.id;
        currentMissionText = m.text;
        hasActiveMission = true;
        if (currentMissionId == "look") lookSum = 0f;

        if (missionText)
        {
            missionText.text = currentMissionText;
            missionText.transform.SetAsLastSibling();
            StopCoroutine(nameof(HideMissionTextFlow));
            StartCoroutine(HideMissionTextFlow());
        }

        if (missionIcon)
        {
            missionIcon.transform.SetAsLastSibling();
            missionIcon.SetActive(m.showIconNow);
        }
    }

    IEnumerator HideMissionTextFlow()
    {
        missionText.gameObject.SetActive(true);
        yield return FadeTMP(missionText, 1f, fadeIn);
        yield return new WaitForSecondsRealtime(missionTextShowSeconds);
        yield return FadeTMP(missionText, 0f, fadeOut);
    }

    public void ShowMissionAgain()
    {
        if (!hasActiveMission || !missionText) return;
        StopCoroutine(nameof(HideMissionTextFlow));
        StartCoroutine(HideMissionTextFlow());
    }

    public void CompleteMission(string id)
    {
        if (!hasActiveMission || currentMissionId != id) return;
        WaitForText("Завдання виконано!");
        currentMissionId = null;
        currentMissionText = null;
        hasActiveMission = false;
        if (missionIcon) missionIcon.SetActive(false);
        if (missionText) { StopCoroutine(nameof(HideMissionTextFlow)); StartCoroutine(FadeTMP(missionText, 0f, fadeOut)); }
        StartNextMission();
    }
}
