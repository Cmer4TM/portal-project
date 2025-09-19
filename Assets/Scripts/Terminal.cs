using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Terminal : MonoBehaviour
{
    public Animator door;
    public Light[] lights;
    public AudioClip pressSound;

    AudioSource audioSource;

    void Awake() => audioSource = GetComponent<AudioSource>();

    public void TerminalActivated()
    {
        audioSource.PlayOneShot(pressSound);

        door.ResetTrigger("Open");
        door.SetBool("Unlocked", true);

        LightColor(Color.green);
    }

    void LightColor(Color color)
    {
        if (lights == null || lights.Length == 0) return;

        foreach (Light light in lights) light.color = color;
    }
}
