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
        public bool completed;
    }

    public RectTransform missionTextBg;
    public TMP_Text missionText;
    public List<Mission> missions;
    public UnityEvent<string> playerTextEvent;

    [SerializeField] float missionTextTime;
    [SerializeField] float missionSwitchTime;
    [SerializeField] float fadeTime;
    [SerializeField] float moveTime;

    Coroutine workerCoroutine;
    InputAction lookAction;

    Vector2 lookSum;

    void Awake() => lookAction = GetComponent<PlayerInput>().actions["Look"];

    void Start()
    {
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
                lookSum += new Vector2(Mathf.Abs(d.x), MathF.Abs(d.y));

                if (lookSum.magnitude >= 5000) StartCoroutine(CompleteMission("look", "Треба знайти вихід..."));

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

    IEnumerator TextFade(float target)
    {
        float elapsedTime = 0;
        float startingAlpha = missionText.alpha;

        while (elapsedTime < fadeTime)
        {
            missionText.alpha = Mathf.Lerp(startingAlpha, target, elapsedTime / fadeTime);
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
            yield return TextFade(0);

            missions.RemoveAt(0);
            missionText.text = missions[0].text;
            missionText.fontStyle = FontStyles.Normal;

            Canvas.ForceUpdateCanvases();
            missionTextBg.sizeDelta = missionText.rectTransform.sizeDelta + new Vector2(40, 0);
            missionTextBg.anchoredPosition = new(-missionTextBg.rect.width, 0);

            yield return TextFade(1);
        }

        yield return new WaitForSeconds(missionTextTime);
        yield return TextMove(0, moveTime);

        workerCoroutine = null;
    }

    IEnumerator CompleteMission(string missionId, string playerText = null)
    {
        if (missions[0].id != missionId) yield break;

        missions[0].completed = true;

        yield return workerCoroutine;
        workerCoroutine = StartCoroutine(ShowAndHideMission(true));

        if (string.IsNullOrEmpty(playerText) == false) playerTextEvent?.Invoke(playerText);
    }
}
