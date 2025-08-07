using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

public class UseButtonHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private TextMeshProUGUI buttonText;

    private Button buttonComponent;
    private Image buttonImage;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private List<TimelineAsset> timelines = new List<TimelineAsset>();

    private void Awake()
    {
        buttonComponent = buttonObject.GetComponent<Button>();
        buttonImage = buttonObject.GetComponent<Image>();
        HideButton();
    }

    public void ShowButton()
    {
        ShowButton("Встати");
    }

    public void ShowButton(string label)
    {
        buttonComponent.interactable = true;
        buttonImage.enabled = true;
        buttonText.enabled = true;
        buttonText.text = label;
    }

    public void HideButton()
    {
        buttonComponent.interactable = false;
        buttonImage.enabled = false;
        buttonText.enabled = false;
    }

    public void PlayTimeline(int index)
    {
        director.playableAsset = timelines[index];
        director.Play();
    }
}
