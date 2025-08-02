using UnityEngine;
using UnityEngine.UI;

public class PauseSettingsConnector : MonoBehaviour
{
    public Slider brightnessSlider;
    public Slider volumeSlider;

    private void Start()
    {
        // Отримати значення з SettingsManager або PlayerPrefs
        float brightness = SettingsManager.Instance != null
            ? SettingsManager.Instance.brightness
            : PlayerPrefs.GetFloat("Brightness", 0.5f);

        float volume = SettingsManager.Instance != null
            ? SettingsManager.Instance.volume
            : PlayerPrefs.GetFloat("Volume", 1f);

        // Призначити значення слайдерам
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.value = brightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Ще раз явно застосувати яскравість і гучність
        ApplySettings(brightness, volume);
    }

    private void ApplySettings(float brightness, float volume)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetBrightness(brightness);
            SettingsManager.Instance.SetVolume(volume);
        }
    }

    public void SetBrightness(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetBrightness(value);
    }

    public void SetVolume(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetVolume(value);
    }
}
