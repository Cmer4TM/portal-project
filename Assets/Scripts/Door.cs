using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public UnityEvent<string> playerTextEvent;

    public void DoorClosed(string playerText) => playerTextEvent?.Invoke(playerText);
}
