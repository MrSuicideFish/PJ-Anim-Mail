using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    public Rigidbody2D m_rigidBody;
    public Animator m_animator;
    public Transform m_view;
    
    [Header("Sprite")]
    public float ViewRotationSpeed = 5.0f;
    
    [Header("Ground Check")]
    public LayerMask m_groundLayer;
    public float m_groundCheckYOffset;
    public float m_groundCheckXOffset;
    public float m_checkDistance = 1.0f;

    [Header("Locomotion")] 
    public float m_moveSpeed;
    public float m_jumpForce;

    private Vector3 m_moveDirection;
    private bool m_isGrounded;
    private bool m_hasJumped;
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
        Vector3 center = transform.position;
        center.y += m_groundCheckYOffset;

        Vector3 left = center, right = center;
        right.x += m_groundCheckXOffset;
        left.x -= m_groundCheckXOffset;

        m_isGrounded = Physics2D.Raycast(left,
                           Vector3.down, m_checkDistance, m_groundLayer) ||
                       Physics2D.Raycast(right,
                           Vector3.down, m_checkDistance, m_groundLayer);
	if(m_isGrounded)
	{
		//m_animator.ResetTrigger("hasJumped");
	}
    }

    private void DoGroundMove()
    {
        m_rigidBody.linearVelocity = new Vector2(m_moveDirection.x * m_moveSpeed, m_rigidBody.linearVelocityY);
    }

    private void Update()
    {
        InputSystem.Update();
        CheckGrounded();
        HandleFacingDirection();

        if (m_isGrounded)
        {
            DoGroundMove();
        }
        
        m_animator.SetFloat("Velocity", m_moveDirection.normalized.magnitude);
        m_animator.SetBool("IsGrounded", m_isGrounded);

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (m_isGrounded)
            {
                m_rigidBody.AddForce(Vector2.up * m_jumpForce, ForceMode2D.Impulse);
        	m_animator.SetTrigger("hasJumped");
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveDirection = context.ReadValue<Vector2>();
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;
        center.y += m_groundCheckYOffset;

        Vector3 left = center, right = center;
        right.x += m_groundCheckXOffset;
        left.x -= m_groundCheckXOffset;

        
        Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(left, size);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(right, size);
    }
}