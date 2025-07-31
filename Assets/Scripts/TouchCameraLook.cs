using UnityEngine;
using UnityEngine.EventSystems;

public class TouchCameraLook : MonoBehaviour
{
    public float sensitivity = 0.1f;
    [Range(0f, 1f)] public float rightScreenThreshold = 0.6f;

    public Transform cameraHolder; // -> цей об’єкт має обертатися по X (вгору-вниз)

    private float pitch = 0f;
    private float yaw = 0f;
    private int cameraTouchId = -1;
    private Vector2 lastTouchPosition;
    private bool isDragging = false;

    void Start()
    {
        // Встановлюємо початкові кути
        yaw = transform.eulerAngles.y;
        pitch = cameraHolder.localEulerAngles.x;
    }

    void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                continue;

            Vector2 pos = touch.position;
            if (pos.x < Screen.width * rightScreenThreshold)
                continue;

            if (cameraTouchId == -1 && touch.phase == TouchPhase.Began)
            {
                cameraTouchId = touch.fingerId;
                lastTouchPosition = pos;
                isDragging = true;
            }

            if (touch.fingerId == cameraTouchId)
            {
                if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector2 delta = pos - lastTouchPosition;
                    lastTouchPosition = pos;

                    yaw += delta.x * sensitivity;
                    pitch -= delta.y * sensitivity;
                    pitch = Mathf.Clamp(pitch, -60f, 60f);

                    transform.rotation = Quaternion.Euler(0f, yaw, 0f); // Player обертається по Y
                    cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f); // Камера — по X
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                    cameraTouchId = -1;
                }
            }
        }
    }
}
