using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    public Rigidbody2D m_rigidBody;
    public Transform m_view;
    
    [Header("Sprite")]
    public float ViewRotationSpeed = 5.0f;
    
    [Header("Ground Check")]
    public Vector3 m_checkHeightOffset;
    public float m_checkDistance = 1.0f;

    private Vector3 m_moveDirection;
    private bool m_isGrounded;
    private bool m_isJumping;
    private bool m_isGliding;
    private bool m_isFacingRight;
    
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

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position + m_checkHeightOffset, Vector3.down);
        m_isGrounded = Physics.Raycast(ray, out RaycastHit hit, m_checkDistance, LayerMask.GetMask("Default"));
    }

    private void Update()
    {
        InputSystem.Update();
        CheckGrounded();
        HandleFacingDirection();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (m_isGrounded)
            {
                m_isJumping = true;
                m_isGliding = false;
            }
            else
            {
                m_isGliding = true;
            }
        }
        else if (context.canceled)
        {
            m_isGliding = false;
            m_isJumping = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveDirection = context.ReadValue<Vector2>();
    }
}