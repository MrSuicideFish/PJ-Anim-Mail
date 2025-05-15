using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform m_view;
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private Animator m_animator;
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
    private float m_jumpReleaseTime;
    private float m_glideTime;

    private void HandleFacingDirection()
    {
        // Flip the player around when changing direction
        if (m_moveDirection.x != 0)
        {
            m_isFacingRight = !(m_moveDirection.x < 0);
        }
        
        Vector3 targetRot = new Vector3(0, m_isFacingRight ? 0 : 180, 0);
        m_view.rotation = Quaternion.Slerp(m_view.rotation, Quaternion.Euler(targetRot), ViewRotationSpeed * Time.deltaTime);
    }

    private void Update()
    {
        InputSystem.Update();
        HandleFacingDirection();
        
        m_isGrounded = m_characterController.isGrounded;
        
        // horizontal movement
        Vector3 move = new Vector3(m_moveDirection.x, 0, m_moveDirection.y);
        m_characterController.Move(move * moveSpeed * Time.deltaTime);
        
        // Check for jump peak and enable gliding if holding jump
        if (!m_isGrounded && m_velocity.y <= 0 && m_isJumping)
        {
            m_isGliding = true;
        }
        
        // Apply gravity
        if (m_isGliding)
        {
            m_glideTime = Mathf.Clamp01(m_glideTime + Time.deltaTime);
            m_velocity.y += glideGravity * Time.deltaTime * (1.0f + (m_glideTime * (fallMultiplier * 2.0f)));
        }
        else if (!m_isGrounded && !m_isJumping)
        {
            m_jumpReleaseTime = Mathf.Clamp01(m_jumpReleaseTime + Time.deltaTime * fallMultiplier);
            float multiplier = fallMultiplier * m_jumpReleaseTime;
            m_velocity.y += gravity * multiplier * Time.deltaTime;
        }
        else
        {
            m_velocity.y += gravity * Time.deltaTime;
        }

        m_characterController.Move(m_velocity * Time.deltaTime);

        m_animator.SetFloat("Velocity", move.normalized.magnitude);
        m_animator.SetBool("IsGrounded", m_isGrounded);
        m_animator.SetTrigger("hasJumped");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (m_isGrounded)
            {
                m_isJumping = true;
                m_isGliding = false;
                m_velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
            else
            {
                m_isGliding = true;
            }
        }
        else if (context.canceled)
        {
            m_jumpReleaseTime = 0.0f;
            m_isGliding = false;
            m_isJumping = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveDirection = context.ReadValue<Vector2>();
    }
}