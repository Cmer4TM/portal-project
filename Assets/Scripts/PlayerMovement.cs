using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(CapsuleCollider), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private const float GRAVITY = -9.81f;

    private PlayerInput playerInput;
    private CharacterController controller;
    public Transform cameraTransform;

    private InputAction moveAction;
    //private InputAction lookAction;
    private InputAction jumpAction;

    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce = 5;
    //[SerializeField] private float mouseSensitivity = 3;

    private float xRotation;
    private float fallSpeed;

    void Awake()
    {
        Application.targetFrameRate = 120;

        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        moveAction = playerInput.actions["Move"];
        //lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        //Rotation();
        Movement();
    }

    //void Rotation()
    //{
    //    Vector2 look = mouseSensitivity * Time.deltaTime * lookAction.ReadValue<Vector2>();

    //    xRotation -= look.y;
    //    xRotation = Mathf.Clamp(xRotation, -90, 90);
    //    cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

    //    transform.Rotate(Vector3.up * look.x);
    //}

    void Movement()
    {
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
