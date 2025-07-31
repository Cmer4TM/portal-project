using UnityEngine;
using UnityEngine.EventSystems;

public class LookAreaTouch : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform playerTf;
    [SerializeField] private Transform cameraTf;
    [SerializeField] private float sensitivity = 0.1f;

    private float xRotation;
    private Vector2 lastPosition;
    private bool dragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging == false) return;

        Vector2 currentPosition = eventData.position;
        Vector2 delta = currentPosition - lastPosition;
        lastPosition = currentPosition;

        xRotation -= delta.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        cameraTf.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerTf.Rotate(delta.x * sensitivity * Vector3.up);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
