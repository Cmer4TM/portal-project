using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public float brightness = 0.5f;
    public float volume = 1f;

    public Image brightnessOverlay;
    public Slider brightnessSlider;
    public Slider volumeSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            brightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
            volume = PlayerPrefs.GetFloat("Volume", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyBrightness();
        ApplyVolume();
        TryReconnectSliders();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindOverlayAgain();
        TryReconnectSliders();
        ApplyBrightness();
        ApplyVolume();
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

    private void FindOverlayAgain()
    {
        if (brightnessOverlay == null)
        {
            GameObject overlay = GameObject.Find("BrightnessOverlay");
            if (overlay != null)
                brightnessOverlay = overlay.GetComponent<Image>();
        }
    }

    private void TryReconnectSliders()
    {
        Debug.Log("🔁 Reconnecting sliders...");

        var bsObj = GameObject.Find("BrightnessSlider");
        if (bsObj != null)
        {
            brightnessSlider = bsObj.GetComponent<Slider>();
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.value = brightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            Debug.Log("✅ Brightness slider connected");
        }
        else
        {
            Debug.LogWarning("⚠️ BrightnessSlider not found");
        }

        var vsObj = GameObject.Find("VolumeSlider");
        if (vsObj != null)
        {
            volumeSlider = vsObj.GetComponent<Slider>();
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
            Debug.Log("✅ Volume slider connected");
        }
        else
        {
            Debug.LogWarning("⚠️ VolumeSlider not found");
        }
    }

    public void ResetSettings()
    {
        SetBrightness(0.5f);
        SetVolume(1f);

        if (brightnessSlider != null) brightnessSlider.value = brightness;
        if (volumeSlider != null) volumeSlider.value = volume;
    }
}
