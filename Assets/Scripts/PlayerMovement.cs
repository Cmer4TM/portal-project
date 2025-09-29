using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float stepDistance;

    PlayerInput playerInput;
    CharacterController controller;
    Animator animator;
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
        animator = GetComponent<Animator>();

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
                AudioManager.Instance.sfxAudioSource.PlayOneShot(AudioManager.Instance.jumpSound);
            }
            else fallSpeed = -Mathf.Sqrt(-GRAVITY);

            if (wasGrounded == false)
            {
                AudioManager.Instance.sfxAudioSource.PlayOneShot(AudioManager.Instance.landSound);
            }
        }
        else fallSpeed += GRAVITY * Time.deltaTime;

        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        if (direction.magnitude > 1) direction.Normalize();

        animator.SetFloat("Speed", direction.magnitude, 0.1f, Time.deltaTime);

        direction *= moveSpeed * Time.deltaTime;

        controller.Move(direction + fallSpeed * Time.deltaTime * Vector3.up);

        if (controller.isGrounded)
        {
            distanceSinceLastStep += direction.magnitude;

            if (distanceSinceLastStep > stepDistance)
            {
                int index = Random.Range(0, AudioManager.Instance.footstepClips.Length);
                AudioManager.Instance.sfxAudioSource.PlayOneShot(AudioManager.Instance.footstepClips[index]);

                distanceSinceLastStep = 0;
            }
        }

        wasGrounded = controller.isGrounded;
    }
}
