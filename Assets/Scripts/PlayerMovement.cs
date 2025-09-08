using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;

    public float moveSpeed;
    public float jumpForce;

    PlayerInput playerInput;
    CharacterController controller;

    InputAction moveAction;
    InputAction jumpAction;

    float fallSpeed;

    const float GRAVITY = -9.81f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        if (controller.enabled == false) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        fallSpeed = controller.isGrounded
            ? jumpAction.triggered
                ? jumpForce
                : -Mathf.Sqrt(-GRAVITY)
            : fallSpeed + GRAVITY * Time.deltaTime;

        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        if (direction.magnitude > 1) direction.Normalize();
        direction *= moveSpeed * Time.deltaTime;

        controller.Move(direction + fallSpeed * Time.deltaTime * Vector3.up);
    }
}
