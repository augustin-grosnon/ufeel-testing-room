using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float _speed = 4f;
    public float _jumpForce = 5f;

    private Rigidbody _rb;
    private bool _isGrounded = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
    }

    private void MovePlayer()
    {
        float moveDirection = Input.GetAxis("Vertical") * _speed;
        float strafeDirection = Input.GetAxis("Horizontal") * _speed;

        Vector3 move = transform.forward * moveDirection + transform.right * strafeDirection;

        Vector3 velocity = new(move.x, _rb.linearVelocity.y, move.z);
        _rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            _isGrounded = true;
        }
    }
}
