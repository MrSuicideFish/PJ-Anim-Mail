using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform m_view;
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float glideGravity = -2f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float ViewRotationSpeed = 2.5f;

    private Vector2 m_moveDirection;
    private Vector3 m_velocity;
    private bool m_isGrounded;
    private bool m_isJumping;
    private bool m_isGliding;
    private bool m_isFacingRight = true;

    private void Update()
    {
        // flip the player around when changing direction
        if (m_moveDirection.x != 0)
        {
            m_isFacingRight = !(m_moveDirection.x < 0);
        }

        Vector3 targetRot = new Vector3(0, m_isFacingRight ? 0 : 180, 0);
        m_view.rotation = Quaternion.Slerp(m_view.rotation, Quaternion.Euler(targetRot), ViewRotationSpeed * Time.deltaTime);
        
        // Check if the player is grounded
        m_isGrounded = m_characterController.isGrounded;

        if (m_isGrounded && m_velocity.y < 0)
        {
            m_velocity.y = 0f; // Reset vertical velocity when grounded
        }

        // Horizontal movement
        Vector3 move = new Vector3(m_moveDirection.x, 0, m_moveDirection.y);
        m_characterController.Move(move * moveSpeed * Time.deltaTime);

        // Apply gravity
        if (m_isGliding)
        {
            m_velocity.y += glideGravity * Time.deltaTime;
        }
        else if (!m_isGrounded && !m_isJumping)
        {
            m_velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            m_velocity.y += gravity * Time.deltaTime;
        }

        // Apply vertical velocity
        m_characterController.Move(m_velocity * Time.deltaTime);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && m_isGrounded)
        {
            m_isJumping = true;
            m_velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Apply jump force
        }
        else if (context.canceled)
        {
            m_isJumping = false;
            m_isGliding = false; // Stop gliding when jump is released
        }
        else if (context.performed && !m_isGrounded)
        {
            m_isGliding = true; // Start gliding when holding jump
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveDirection = context.ReadValue<Vector2>();
    }
}