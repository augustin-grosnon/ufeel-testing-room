using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private DoorScript.Door door;

    public void ToggleDoor()
    {
        if (door != null)
        {
            door.ToggleDoor();
        }
    }
}
