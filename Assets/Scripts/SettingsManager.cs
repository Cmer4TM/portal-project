using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider brightnessSlider;
    public Slider volumeSlider;

    public Image brightnessOverlay;

    const string Brightness = nameof(Brightness);
    const string Volume = nameof(Volume);

    void Start()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat(Brightness, 1);
        volumeSlider.value = PlayerPrefs.GetFloat(Volume, 1);
    }

    public void SetBrightness(float value)
    {
        brightnessOverlay.color = new(0, 0, 0, Mathf.Lerp(0, 0.6f, 1 - value));

        PlayerPrefs.SetFloat(Brightness, value);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat(Volume, value);
        PlayerPrefs.Save();
    }
}
