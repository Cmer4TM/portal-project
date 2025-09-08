using UnityEngine;
using UnityEngine.EventSystems;

public class LookAreaTouch : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Transform playerTransform;
    public Transform cameraTransform;

    public float sensitivity;

    Vector2 lastPosition;
    float xRotation;

    public void OnPointerDown(PointerEventData e) => lastPosition = e.position;

    public void OnDrag(PointerEventData e)
    {
        Vector2 delta = e.position - lastPosition;
        lastPosition = e.position;

        xRotation -= delta.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerTransform.Rotate(delta.x * sensitivity * Vector3.up);
    }
}
