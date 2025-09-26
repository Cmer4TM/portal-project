using UnityEngine;

public class Machine : MonoBehaviour
{
    public GameObject fixedMachine;

    public void Interact()
    {
        fixedMachine.SetActive(true);
        Destroy(gameObject);
    }
}
