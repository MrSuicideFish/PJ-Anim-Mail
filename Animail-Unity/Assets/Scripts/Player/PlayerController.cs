using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform m_view;
    [SerializeField] private Rigidbody m_rigidBody;
    private Vector2 m_moveDirection;
    private bool m_isFacingRight;
    private bool m_hasJumped;
    private bool m_isGrounded;
    
    public float MoveSpeed = 5f;
    public float ViewRotationSpeed = 0.1f;

    private void Update()
    {
        // flip the player around when changing direction
        if (m_moveDirection.x != 0)
        {
            m_isFacingRight = !(m_moveDirection.x < 0);
        }

        Vector3 targetRot = new Vector3(0, m_isFacingRight ? 0 : 180, 0);
        m_view.rotation = Quaternion.Slerp(m_view.rotation, Quaternion.Euler(targetRot), ViewRotationSpeed * Time.deltaTime);

        if (m_hasJumped)
        {
            //velocity.y = 10;
        }
        
        if (m_isGrounded)
        {

        }
        
        Vector3 move = transform.right * m_moveDirection.x + transform.forward * m_moveDirection.y;
        m_rigidBody.AddForce(move * MoveSpeed, ForceMode.VelocityChange);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            m_hasJumped = true;
        }else if(context.canceled)
        {
            m_hasJumped = false;
        }
    }

    public void OnInteract()
    {
        
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveDirection = context.ReadValue<Vector2>();
    }
}
