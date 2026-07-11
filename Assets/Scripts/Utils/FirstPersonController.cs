using UFeel;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Look Settings")]
    public Transform cameraRoot;
    public float mouseSensitivity = 2f;
    private float pitch = 0f;
    private bool isUsingMouseToMoveView = true;

    [Header("Gravity")]
    public float gravity = -9.81f;
    private float verticalVelocity;
    public CharacterController Controller;

    private async void Start()
    {
        Controller = gameObject.AddComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        await UFeelAPI.StartAPI();
    }

    private void Update()
    {
        HandleMovement();

        if (isUsingMouseToMoveView)
        {
            HandleMouseLook();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCursor();
        }
    }

    private void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isUsingMouseToMoveView = !isUsingMouseToMoveView;
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * moveX) + (transform.forward * moveZ);
        verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;

        Controller.Move(moveSpeed * Time.deltaTime * move);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
