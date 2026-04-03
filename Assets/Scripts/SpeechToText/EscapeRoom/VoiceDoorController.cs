using UnityEngine;

public class VoiceDoorController : MonoBehaviour
{
    public Transform pivot;
    public float openAngle = 90f;
    public float speed = 2f;

    private bool isOpen = false;

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }

    void Update()
    {
        float targetY = isOpen ? openAngle : 0f;
        Vector3 targetRotation = new Vector3(0, targetY, 0);

        pivot.localRotation = Quaternion.Lerp(
            pivot.localRotation,
            Quaternion.Euler(targetRotation),
            Time.deltaTime * speed
        );
    }
}
