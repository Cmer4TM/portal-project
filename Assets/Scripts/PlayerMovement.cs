using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float jumpForce = 4f;

    [Header("Footsteps")]
    public AudioClip[] footstepClips;
    public float stepDistance = 2f;

    [Header("Jump / Land Sounds")]
    public AudioClip jumpSound;
    public AudioClip landSound;

    private PlayerInput playerInput;
    private CharacterController controller;
    private AudioSource audioSource;

    private InputAction moveAction;
    private InputAction jumpAction;

    private float fallSpeed;
    private bool wasGrounded;
    private Vector3 lastPosition;
    private float distanceSinceLastStep;

    private const float GRAVITY = -9.81f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Start()
    {
        lastPosition = transform.position;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (!controller.enabled) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        bool justJumped = false;
        if (controller.isGrounded)
        {
            if (jumpAction.triggered)
            {
                fallSpeed = jumpForce;
                justJumped = true;
                PlayJump();
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

        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 direction = right * move.x + forward * move.y;
        if (direction.sqrMagnitude > 1f) direction.Normalize();
        Vector3 horizontalMove = direction * moveSpeed * Time.deltaTime;

        Vector3 before = transform.position;
        controller.Move(horizontalMove + fallSpeed * Time.deltaTime * Vector3.up);
        Vector3 after = transform.position;

        Vector3 delta = after - before;
        delta.y = 0f;
        distanceSinceLastStep += delta.magnitude;

        if (controller.isGrounded && move.sqrMagnitude > 0.1f && distanceSinceLastStep > stepDistance)
        {
            PlayFootstep();
            distanceSinceLastStep = 0f;
        }
        if (!wasGrounded && controller.isGrounded && !justJumped)
        {
            PlayLand();
        }

        wasGrounded = controller.isGrounded;
        lastPosition = after;
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(footstepClips[index]);
    }

    void PlayJump()
    {
        if (jumpSound != null)
            audioSource.PlayOneShot(jumpSound);
    }

    void PlayLand()
    {
        if (landSound != null)
            audioSource.PlayOneShot(landSound);
    }
}
