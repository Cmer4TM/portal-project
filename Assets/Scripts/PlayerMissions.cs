using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMissions : MonoBehaviour
{
    [Serializable]
    public class Mission
    {
        public string id;
        public string text;
        public string finishText;

        [NonSerialized] public bool completed;
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

    void Start()
    {
        missionText.text = missions[0].text;
        workerCoroutine = StartCoroutine(ShowAndHideMission(false));
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

    IEnumerator ShowAndHideMission(bool complete)
    {
        if (complete) missionText.fontStyle = FontStyles.Strikethrough;

        Canvas.ForceUpdateCanvases();
        missionTextBg.sizeDelta = missionText.rectTransform.sizeDelta + new Vector2(40, 0);

        yield return TextMove(-missionTextBg.rect.width, moveTime);

        if (complete)
        {
            yield return new WaitForSeconds(missionSwitchTime);
            yield return GameManager.TextFade(missionText, 0, fadeTime);

            missions.RemoveAt(0);
            missionText.text = missions[0].text;
            missionText.fontStyle = FontStyles.Normal;

            Canvas.ForceUpdateCanvases();
            missionTextBg.sizeDelta = missionText.rectTransform.sizeDelta + new Vector2(40, 0);
            missionTextBg.anchoredPosition = new(-missionTextBg.rect.width, 0);

            yield return GameManager.TextFade(missionText, 1, fadeTime);
        }

        yield return new WaitForSeconds(missionTextTime);
        yield return TextMove(0, moveTime);

        workerCoroutine = null;
    }

    IEnumerator CompleteMission(string missionId, GameObject trigger = null)
    {
        if (missions[0].id != missionId) yield break;

        if (trigger) Destroy(trigger);

        missions[0].completed = true;

        yield return workerCoroutine;
        workerCoroutine = StartCoroutine(ShowAndHideMission(true));

        if (missions[0].finishText != null) playerTextEvent?.Invoke(missions[0].finishText);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mission")) StartCoroutine(CompleteMission(other.name, other.gameObject));
    }
}
