using UnityEngine;
using UnityEngine.EventSystems;

public class LookAreaTouch : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform playerTf;
    [SerializeField] private Transform cameraTf;
    [SerializeField] private float sensitivity = 0.1f;

    private float xRotation;
    private Vector2 lastPosition;

    public void OnPointerDown(PointerEventData e) => lastPosition = e.position;

    public void OnDrag(PointerEventData e)
    {
        Vector2 delta = e.position - lastPosition;
        lastPosition = e.position;

        xRotation -= delta.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        cameraTf.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerTf.Rotate(delta.x * sensitivity * Vector3.up);
    }
}
