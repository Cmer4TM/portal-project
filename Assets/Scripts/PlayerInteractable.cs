using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public Transform playerCamera;
    public GameObject bluePortalPrefab;
    public GameObject redPortalPrefab;

    [SerializeField] private Vector2 portalOffset;
    [SerializeField] private Vector3 playerTeleportOffset;

    GameObject redPortal;
    GameObject bluePortal;

    public void RedPortal() => SpawnPortal(ref redPortal, redPortalPrefab);

    public void BluePortal() => SpawnPortal(ref bluePortal, bluePortalPrefab);

    void SpawnPortal(ref GameObject portal, GameObject prefab)
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit) == false) return;
        if (hit.collider.CompareTag("Placeable") == false) return;

        Bounds bounds = hit.collider.bounds;

        Vector3 localX = Vector3.Cross(hit.normal, Mathf.Abs(hit.normal.y) == 1 ? Vector3.right : Vector3.up);
        Vector3 localY = Vector3.Cross(hit.normal, -localX);

        Vector3 distance = hit.point - bounds.center;

        float localClampedBoundsX = Mathf.Abs(Vector3.Dot(bounds.extents, localX)) - portalOffset.x;
        float localClampedBoundsY = Mathf.Abs(Vector3.Dot(bounds.extents, localY)) - portalOffset.y;

        float localDistanceX = Mathf.Clamp(Vector3.Dot(distance, localX), -localClampedBoundsX, localClampedBoundsX);
        float localDistanceY = Mathf.Clamp(Vector3.Dot(distance, localY), -localClampedBoundsY, localClampedBoundsY);

        Vector3 clampedPosition = bounds.center + localX * localDistanceX + localY * localDistanceY + hit.normal * 0.3f;
        Quaternion rotation = Quaternion.LookRotation(hit.normal);

        if (portal == null)
        {
            portal = Instantiate(prefab, clampedPosition, rotation);
            portal.name = portal.name.Replace("(Clone)", "");

        }
        else portal.transform.SetPositionAndRotation(clampedPosition, rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teleport") == false) return;
        if (bluePortal == null || redPortal == null) return;

        Transform source = other.transform.parent;
        GameObject target = source.name == "Red Portal" ? bluePortal : redPortal;

        Vector3 posOffset = target.transform.TransformDirection(playerTeleportOffset);
        Quaternion rotOffset = Quaternion.Inverse(source.rotation) * Quaternion.Euler(0, 180, 0) * transform.rotation;

        transform.SetPositionAndRotation(target.transform.position + posOffset, target.transform.rotation * rotOffset);
    }
}
