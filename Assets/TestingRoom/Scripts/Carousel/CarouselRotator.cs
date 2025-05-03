using UnityEngine;

public class CarouselRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 0.5f;
    public bool isRotatingContinuously = true;
    public float radius = 14.8f;
    public Transform doorHoldersParent;

    [Header("Target Rotation")]
    public bool rotateToTarget = false;
    public float targetAngle = 0f;
    public float rotationSmoothSpeed = 90f;

    private Transform[] doorHolders;

    void Start()
    {
        doorHolders = new Transform[doorHoldersParent.childCount];

        int i = 0;
        foreach (Transform child in doorHoldersParent)
        {
            if (child.CompareTag("DoorHolder"))
            {
                doorHolders[i] = child;
                i++;
            }
        }
    }

    void Update()
    {
        if (isRotatingContinuously)
        {
            RotateCarousel(rotationSpeed * Time.deltaTime);
        }

        if (rotateToTarget)
        {
            RotateToAngle(targetAngle);
        }

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isRotatingContinuously = !isRotatingContinuously;
        }
    }

    public void RotateCarousel(float amount)
    {
        // transform.Rotate(Vector3.up, amount, Space.Self);

        for (int i = 0; i < doorHolders.Length; i++)
        {
            float angle = i * (360f / doorHolders.Length) + (rotationSpeed * Time.time * 360f / 20f);
            float angleRad = Mathf.Deg2Rad * angle;
            float x = Mathf.Sin(angleRad) * radius;
            float z = Mathf.Cos(angleRad) * radius;

            doorHolders[i].localPosition = new Vector3(x, doorHolders[i].localPosition.y, z);
        }
    }

    public void SetIsRotating(bool shouldRotate)
    {
        isRotatingContinuously = shouldRotate;
    }

    public void RotateToAngle(float angle)
    {
        float currentY = transform.localEulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(currentY, angle, rotationSmoothSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0f, newY, 0f);

        if (Mathf.Approximately(newY, angle))
        {
            rotateToTarget = false;
        }
    }

    public void TriggerRotateTo(float angle)
    {
        targetAngle = angle;
        rotateToTarget = true;
        isRotatingContinuously = false;
    }
}
