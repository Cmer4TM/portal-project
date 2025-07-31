using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PlayerInput playerInput;
    public Transform cameraTransform;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private int mouseSensitivity = 100;

    private float xRotation;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        Vector2 mouse = mouseSensitivity * Time.deltaTime * lookAction.ReadValue<Vector2>();

        xRotation -= mouse.y;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * mouse.x);

        if (jumpAction.triggered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float maxDistance = capsuleCollider.bounds.extents.y + 0.01f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out _, maxDistance);

        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * move.x + transform.forward * move.y;
        
        Vector3 newPos = rb.position + moveSpeed * Time.fixedDeltaTime * moveDirection;
        rb.MovePosition(newPos);
    }
}
