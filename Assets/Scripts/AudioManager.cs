using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public static AudioSource AudioSource { get; private set; }

    public AudioSource musicAudioSource;

    public AudioClip buttonSound;
    public AudioClip navigateSound;
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;

    void Awake()
    {
        Instance = this;
        AudioSource = GetComponent<AudioSource>();

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        musicAudioSource.clip = scene.name switch
        {
            "MainMenuUIScene" => mainMenuMusic,
            _ => gameMusic
        };

        musicAudioSource.Play();
    }
}
