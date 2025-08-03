using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    private Image brightnessOverlay;
    private float brightness = 0.5f;
    private float volume = 1f;

    private Slider brightnessSlider;
    private Slider volumeSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        brightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        volume = PlayerPrefs.GetFloat("Volume", 1f);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindOverlay();
        ApplyBrightness();
        ApplyVolume();
    }

    private void TryFindOverlay()
    {
        GameObject overlayObj = GameObject.Find("BrightnessOverlay");

        if (overlayObj != null)
        {
            brightnessOverlay = overlayObj.GetComponent<Image>();

            if (brightnessOverlay == null)
            {
                Debug.LogWarning("BrightnessOverlay found, but it doesn't have an Image component.");
            }
        }
        else
        {
            Debug.LogWarning("BrightnessOverlay not found in the scene.");
        }
    }

    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("Brightness", brightness);
        ApplyBrightness();
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("Volume", volume);
        ApplyVolume();
    }

    private void ApplyBrightness()
    {
        if (brightnessOverlay != null)
        {
            Color c = brightnessOverlay.color;
            c.a = Mathf.Lerp(0f, 0.6f, 1f - brightness);
            brightnessOverlay.color = c;
        }
    }

    private void ApplyVolume()
    {
        AudioListener.volume = volume;
    }

    public void AssignSliders(Slider newBrightnessSlider, Slider newVolumeSlider)
    {
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveAllListeners();
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveAllListeners();

        brightnessSlider = newBrightnessSlider;
        volumeSlider = newVolumeSlider;

        if (brightnessSlider != null)
        {
            brightnessSlider.value = brightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        ApplyBrightness();
        ApplyVolume();
    }
}
