using UnityEngine;

public class Machine : MonoBehaviour
{
    public GameObject fixedMachine;
    public Transform portalGun;
    public Transform portalGunParent;

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
    }
}
