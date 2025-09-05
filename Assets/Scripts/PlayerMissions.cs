using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMissions : MonoBehaviour
{
    [Serializable]
    public class Mission
    {
        public string id;
        public string text;
        [NonSerialized] public bool completed;
    }

    public RectTransform missionTextBg;
    public TMP_Text missionText;
    public List<Mission> missions;
    public UnityEvent<string> playerTextEvent;

    [SerializeField] float missionTextTime = 5;
    [SerializeField] float missionSwitchTime = 2;
    [SerializeField] float fadeTime = 0.6f;
    [SerializeField] float moveTime = 0.6f;

    Coroutine workerCoroutine;
    InputAction lookAction;
    float lookSum;

    void Start()
    {
        lookAction = GetComponent<PlayerInput>().actions["Look"];

        missionText.text = missions[0].text;
        workerCoroutine = StartCoroutine(ShowAndHideMission(false));
    }

    void Update()
    {
        if (missions[0].completed) return;

        switch (missions[0].id)
        {
            case "look":
                Vector2 d = lookAction.ReadValue<Vector2>();
                lookSum += Mathf.Abs(d.x);

                if (lookSum >= 5000)
                {
                    StartCoroutine(CompleteMission("look"));
                    playerTextEvent?.Invoke("Треба знайти вихід...");
                }

                break;
        }
    }

    public void MissionButton() => workerCoroutine ??= StartCoroutine(ShowAndHideMission(false));

    IEnumerator TextMove(float target, float seconds)
    {
        float elapsedTime = 0;
        Vector2 startingPos = missionTextBg.anchoredPosition;

        while (elapsedTime < seconds)
        {
            missionTextBg.anchoredPosition = Vector2.Lerp(startingPos, new(target, 0), elapsedTime / seconds);
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        missionTextBg.anchoredPosition = new(target, 0);
    }

    IEnumerator TextFade(float target, float seconds)
    {
        float elapsedTime = 0;
        float startingAlpha = missionText.alpha;

        while (elapsedTime < seconds)
        {
            missionText.alpha = Mathf.Lerp(startingAlpha, target, elapsedTime / seconds);
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        missionText.alpha = target;
    }

    IEnumerator ShowAndHideMission(bool complete)
    {
        if (complete) missionText.fontStyle = FontStyles.Strikethrough;

        Canvas.ForceUpdateCanvases();
        missionTextBg.sizeDelta = missionText.rectTransform.sizeDelta + new Vector2(40, 0);

        yield return TextMove(-missionTextBg.rect.width, moveTime);

        if (complete)
        {
            yield return new WaitForSeconds(missionSwitchTime);
            yield return TextFade(0, fadeTime);

            missions.RemoveAt(0);
            missionText.text = missions[0].text;
            missionText.fontStyle = FontStyles.Normal;

            Canvas.ForceUpdateCanvases();
            missionTextBg.sizeDelta = missionText.rectTransform.sizeDelta + new Vector2(40, 0);
            missionTextBg.anchoredPosition = new(-missionTextBg.rect.width, 0);

            yield return TextFade(1, fadeTime);
        }

        yield return new WaitForSeconds(missionTextTime);
        yield return TextMove(0, moveTime);

        workerCoroutine = null;
    }

    IEnumerator CompleteMission(string missionId)
    {
        if (missions[0].id != missionId) yield break;

        missions[0].completed = true;

        yield return workerCoroutine;
        workerCoroutine = StartCoroutine(ShowAndHideMission(true));
    }
}
