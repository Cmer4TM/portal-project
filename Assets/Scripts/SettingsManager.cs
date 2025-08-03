using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Overlay")]
    [SerializeField] private Image brightnessOverlay;

    private const string BrightnessKey = "Brightness";
    private const string VolumeKey = "Volume";

    private void Awake()
    {
        ApplyBrightness(PlayerPrefs.GetFloat(BrightnessKey, 1f));
        ApplyVolume(PlayerPrefs.GetFloat(VolumeKey, 1f));
    }

    public void SetBrightness(float value)
    {
        value = Mathf.Clamp01(value);
        ApplyBrightness(value);
        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        ApplyVolume(value);
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(float value)
    {
        // 0 = яскраво, 0.6 = найтемніше
        var color = brightnessOverlay.color;
        color.a = Mathf.Lerp(0f, 0.6f, 1f - value);
        brightnessOverlay.color = color;
    }

    private void ApplyVolume(float value) => AudioListener.volume = value;

    public void AssignSliders(Slider brightnessSlider, Slider volumeSlider)
    {
        if (brightnessSlider)
        {
            float brightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);
            brightnessSlider.SetValueWithoutNotify(brightness);
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        if (volumeSlider)
        {
            float volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
            volumeSlider.SetValueWithoutNotify(volume);
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }
}
