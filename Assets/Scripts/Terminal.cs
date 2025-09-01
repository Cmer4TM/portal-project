using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Terminal : MonoBehaviour
{
    public Animator door;

    public void TerminalActivated()
    {
        door.ResetTrigger("Open");
        door.SetBool("Unlocked", true);
    }
}
