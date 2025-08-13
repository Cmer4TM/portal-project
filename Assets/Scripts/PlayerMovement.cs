using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private const float GRAVITY = -9.81f;

    private PlayerInput playerInput;
    private CharacterController controller;

    public Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float jumpForce = 5;

    private float fallSpeed;

    void Awake()
    {
        Application.targetFrameRate = 120;

        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        if (controller.enabled == false) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        if (controller.isGrounded)
        {
            if (jumpAction.triggered)
            {
                fallSpeed = jumpForce;
            }
            else
            {
                fallSpeed = -Mathf.Sqrt(-GRAVITY);
            }
        }
        else
        {
            fallSpeed += GRAVITY * Time.deltaTime;
        }

        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        direction = moveSpeed * Time.deltaTime * direction.normalized;

        controller.Move(direction + fallSpeed * Time.deltaTime * Vector3.up);
    }
}
