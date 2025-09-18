using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Terminal : MonoBehaviour
{
    public Animator door;
    public Light[] lights;

    public void TerminalActivated()
    {
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
