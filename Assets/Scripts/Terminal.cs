using UnityEngine;

public class Terminal : MonoBehaviour
{
    public Animator door;
    public Light[] lights;

    public void TerminalActivated()
    {
        AudioManager.Instance.sfxAudioSource.PlayOneShot(AudioManager.Instance.terminalSound);

        foreach (AnimatorControllerParameter parameter in door.parameters)
        {
            if (parameter.name == "Open" && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                door.ResetTrigger("Open");
            }
        }

        door.SetBool("Unlocked", true);

        LightColor(Color.green);
    }

    void LightColor(Color color)
    {
        if (lights == null || lights.Length == 0) return;

        foreach (Light light in lights) light.color = color;
    }
}
