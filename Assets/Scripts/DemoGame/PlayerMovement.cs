using UnityEngine;
using System.Collections.Generic;

public enum EyeHorizontalDirection { Left, Right, None }

public class EyeRecord
{
    public float timeStamp;
    public EyeHorizontalDirection direction;
    public EyeRecord(float t, EyeHorizontalDirection d)
    {
        timeStamp = t;
        direction = d;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float _speed = 2f;
    public float _jumpForce = 5f;
    public float _forwardMoveDistance = 10f;
    public float _sequenceTimeWindow = 2f;
    public float _inputThreshold = 0.1f;
    public float _stoppingDistance = 0.1f;

    private Rigidbody _rb;
    private bool _isGrounded = true;

    private List<EyeRecord> _eyeHistory = new();
    private Vector3 _targetPosition;
    private bool _isAutomaticMovementActive = false;
    private readonly EyeHorizontalDirection[] _eyeSequence = new EyeHorizontalDirection[]
    {
        EyeHorizontalDirection.Left,
        EyeHorizontalDirection.Right,
        EyeHorizontalDirection.Left,
        EyeHorizontalDirection.Right
    };

    void Awake()
    {
        var _ = EyeTrackingReceiver.Instance;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _targetPosition = transform.position;
    }

    void Update()
    {
        UpdateEyeHistory();
        CheckEyeSequenceForwardMovement();
        MovePlayer();
        HandleJump();
    }

    private void UpdateEyeHistory()
    {
        var data = EyeTrackingReceiver.CurrentEyeData;
        EyeHorizontalDirection currentDir = EyeHorizontalDirection.None;

        if (data.left && !data.right)
            currentDir = EyeHorizontalDirection.Left;
        else if (data.right && !data.left)
            currentDir = EyeHorizontalDirection.Right;

        if (currentDir != EyeHorizontalDirection.None)
        {
            _eyeHistory.Add(new EyeRecord(Time.time, currentDir));
        }

        _eyeHistory.RemoveAll(record => Time.time - record.timeStamp > _sequenceTimeWindow);
    }

    private void CheckEyeSequenceForwardMovement()
    {
        int seqIndex = 0;

        foreach (var record in _eyeHistory)
        {
            if (record.direction == _eyeSequence[seqIndex])
            {
                seqIndex++;
                if (seqIndex >= _eyeSequence.Length)
                {
                    _targetPosition = transform.position + transform.forward * _forwardMoveDistance;
                    _isAutomaticMovementActive = true;
                    _eyeHistory.Clear();
                    break;
                }
            }
        }
    }

    private void MovePlayer()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 manualInput = transform.forward * verticalInput + transform.right * horizontalInput;

        if (manualInput.magnitude > _inputThreshold)
        {
            _isAutomaticMovementActive = false;
            _targetPosition = transform.position;
        }

        Vector3 manualVelocity = new(manualInput.x * _speed, _rb.linearVelocity.y, manualInput.z * _speed);

        if (_isAutomaticMovementActive)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            if (distanceToTarget <= _stoppingDistance)
            {
                _isAutomaticMovementActive = false;
            }
            else
            {
                Vector3 directionToTarget = (_targetPosition - transform.position).normalized;
                Vector3 autoVelocity = new(directionToTarget.x * _speed, _rb.linearVelocity.y, directionToTarget.z * _speed);
                _rb.linearVelocity = autoVelocity;
                return;
            }
        }

        _rb.linearVelocity = manualVelocity;
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
