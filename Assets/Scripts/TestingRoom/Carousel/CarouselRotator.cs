using UnityEngine;

public class CarouselRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 0.5f;
    public bool isRotatingContinuously = true;
    public float radius = 19.7f;
    public Transform doorHoldersParent;

    [Header("Target Rotation")]
    public bool rotateToTarget = false;
    public float targetAngle = 0f;
    public float rotationSmoothSpeed = 3f;

    private Transform[] doorHolders;
    private float currentAngle = 0f;

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

        PositionDoors();
    }

    void Update()
    {
        if (rotateToTarget)
        {
            RotateToAngle(targetAngle);
        }
        else if (isRotatingContinuously)
        {
            currentAngle += rotationSpeed * Time.deltaTime * 360f / 20f;
            PositionDoors();
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

    void PositionDoors()
    {
        float angleStep = 360f / doorHolders.Length;

        for (int i = 0; i < doorHolders.Length; i++)
        {
            float angle = i * angleStep + currentAngle;
            float angleRad = Mathf.Deg2Rad * angle;
            float x = Mathf.Sin(angleRad) * radius;
            float z = Mathf.Cos(angleRad) * radius;

            doorHolders[i].localPosition = new Vector3(x, doorHolders[i].localPosition.y, z);
        }
    }

    public void RotateCarousel(float amount)
    {
        currentAngle += amount;
        PositionDoors();
    }

    public void SetIsRotating(bool shouldRotate)
    {
        isRotatingContinuously = shouldRotate;
    }

    public void RotateToAngle(float angle)
    {
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, angle, rotationSmoothSpeed * Time.deltaTime);
        currentAngle = newAngle;
        PositionDoors();

        if (Mathf.Approximately(newAngle, angle))
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

    public void RotateToDoor(int doorIndex)
    {
        float angleStep = 360f / doorHolders.Length;
        float targetDoorAngle = -doorIndex * angleStep;
        TriggerRotateTo(targetDoorAngle);
    }
}
