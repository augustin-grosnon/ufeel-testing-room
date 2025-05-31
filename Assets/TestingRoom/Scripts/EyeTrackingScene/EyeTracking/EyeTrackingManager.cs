using UnityEngine;

public sealed class EyeTrackingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _sphere;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private FloatUpDownInteractionDisabler interactionDisabler;

    [Header("Movement Settings")]
    [SerializeField] private float sphereDistance = 4f;
    [SerializeField] private Vector2 angularMovementRange = new(30f, 20f);
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Gaze Stability Settings")]
    [SerializeField] private float gazeHoldThreshold = 0.25f;
    [SerializeField] private float gazeReleaseCooldown = 0.5f;

    private float gazeTimer = 0f;
    private bool isGazing = false;
    private float timeSinceLostGaze = Mathf.Infinity;

    void Awake()
    {
        var _ = EyeTrackingReceiver.Instance;

        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<Camera>();
            if (playerCamera == null)
                Debug.LogWarning("EyeTrackingManager: No camera found in loaded scenes.");
        }
    }

    void Update()
    {
        if (playerCamera == null || _sphere == null) return;

        UpdateGazeStatus();

        if (!isGazing) return;

        Vector3 targetPosition = ComputeTargetPosition();
        MoveSphere(targetPosition);
    }

    void UpdateGazeStatus()
    {
        if (IsLookingAtSphere())
        {
            gazeTimer += Time.deltaTime;
            timeSinceLostGaze = 0f;

            if (!isGazing && gazeTimer >= gazeHoldThreshold)
            {
                isGazing = true;
                interactionDisabler?.OnInteract();
            }
        }
        else
        {
            gazeTimer = 0f;
            timeSinceLostGaze += Time.deltaTime;

            if (isGazing && timeSinceLostGaze >= gazeReleaseCooldown)
            {
                isGazing = false;
            }
        }
    }

    bool IsLookingAtSphere()
    {
        // Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        // return Physics.Raycast(ray, out RaycastHit hit, 100f) && hit.collider?.gameObject == _sphere.gameObject;

        if (_sphere == null || playerCamera == null)
            return false;

        Vector3 toSphere = _sphere.position - playerCamera.transform.position;
        float angleToSphere = Vector3.Angle(playerCamera.transform.forward, toSphere.normalized);

        float cameraFOV = playerCamera.fieldOfView;

        return angleToSphere < cameraFOV / 2f;
    }

    Vector3 ComputeTargetPosition()
    {
        var data = EyeTrackingReceiver.CurrentEyeData;

        Vector2 direction = Vector2.zero;
        if (data.left) direction.x -= 1;
        if (data.right) direction.x += 1;
        if (data.up) direction.y += 1;
        if (data.down) direction.y -= 1;

        if (direction == Vector2.zero || data.center)
        {
            direction = Vector2.zero;
        }
        else
        {
            direction.Normalize();
        }

        Quaternion offset = Quaternion.Euler(
            direction.y * angularMovementRange.y,
            direction.x * angularMovementRange.x,
            0f
        );

        Vector3 worldDirection = offset * playerCamera.transform.forward;
        return playerCamera.transform.position + worldDirection.normalized * sphereDistance;
    }

    void MoveSphere(Vector3 targetPosition)
    {
        _sphere.position = Vector3.Lerp(_sphere.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
