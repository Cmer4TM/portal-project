using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    public AudioClip buttonSound;
    public AudioClip navigateSound;
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;
    public AudioClip[] footstepClips;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip terminalSound;

    void Awake()
    {
        Instance = Instance ? Instance : this;

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
