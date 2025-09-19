using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public AudioClip jumpSound;
    public AudioClip landSound;

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float stepDistance;

    PlayerInput playerInput;
    CharacterController controller;
    AudioSource audioSource;
    InputAction moveAction;
    InputAction jumpAction;

    float fallSpeed;
    bool wasGrounded;
    float distanceSinceLastStep;

    private const float GRAVITY = -9.81f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

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
                audioSource.PlayOneShot(jumpSound);
            }
            else fallSpeed = -Mathf.Sqrt(-GRAVITY);

            if (wasGrounded == false) audioSource.PlayOneShot(landSound);
        }
        else fallSpeed += GRAVITY * Time.deltaTime;

        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        if (direction.magnitude > 1) direction.Normalize();
        direction *= moveSpeed * Time.deltaTime;

        controller.Move(direction + fallSpeed * Time.deltaTime * Vector3.up);

        if (controller.isGrounded)
        {
            distanceSinceLastStep += direction.magnitude;

            if (distanceSinceLastStep > stepDistance)
            {
                int index = Random.Range(0, footstepClips.Length);
                audioSource.PlayOneShot(footstepClips[index]);

                distanceSinceLastStep = 0;
            }
        }

        wasGrounded = controller.isGrounded;
    }
}
