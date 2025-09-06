using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject mainButtonsPanel;
    public GameObject settingsPanel;
    public GameObject title;

    public Slider brightnessSlider;
    public Slider volumeSlider;

    public Image brightnessOverlay;

    private const string BrightnessKey = "Brightness";
    private const string VolumeKey = "Volume";

    private void Awake()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat(BrightnessKey, 1);
        volumeSlider.value = PlayerPrefs.GetFloat(VolumeKey, 1);
    }

    public void ToggleSettings(bool active)
    {
        if (title) title.SetActive(active == false);
        mainButtonsPanel.SetActive(active == false);
        settingsPanel.SetActive(active);
    }

    public void SetBrightness(float value)
    {
        brightnessOverlay.color = new(0, 0, 0, Mathf.Lerp(0, 0.6f, 1 - value));

        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
}
