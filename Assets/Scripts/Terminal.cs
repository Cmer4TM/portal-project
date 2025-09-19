using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Terminal : MonoBehaviour
{
    public Animator door;
    public Light[] lights;

    [Header("Sound")]
    [SerializeField] private AudioClip pressSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        LightColor(Color.red);
    }

    public void TerminalActivated()
    {
        if (audioSource != null && pressSound != null)
            audioSource.PlayOneShot(pressSound);

        door.ResetTrigger("Open");
        door.SetBool("Unlocked", true);

        LightColor(Color.green);
    }

    void LightColor(Color color)
    {
        if (lights == null || lights.Length == 0) return;

        foreach (Light light in lights)
            light.color = color;
    }
}
