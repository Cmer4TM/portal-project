using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Terminal : MonoBehaviour
{
    public Animator door;
    public Light[] lights;

    void Start() => LightColor(Color.red);

    public void TerminalActivated()
    {
        door.ResetTrigger("Open");
        door.SetBool("Unlocked", true);

        LightColor(Color.green);
    }

    void LightColor(Color color)
    {
        foreach (Light light in lights) light.color = color;
    }
}
