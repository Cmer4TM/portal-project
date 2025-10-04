using UnityEngine;
using UnityEngine.Events;

public class Machine : MonoBehaviour
{
    public GameObject fixedMachine;
    public Transform portalGun;
    public Transform portalGunParent;
    public UnityEvent portalGunEvent;

    public void Fix()
    {
        fixedMachine.SetActive(true);
        fixedMachine.GetComponent<Animator>().SetTrigger("Open");

        Destroy(gameObject);
    }

    public void PortalGun()
    {
        portalGun.parent = portalGunParent;
        portalGun.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        portalGun.localScale = Vector3.one;

        portalGunEvent?.Invoke();
    }
}
